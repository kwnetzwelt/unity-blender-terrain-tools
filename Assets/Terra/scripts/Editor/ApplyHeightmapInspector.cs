using UnityEngine;
using System.Collections;
using UnityEditor;
namespace Terra.Editor
{
	[CustomEditor(typeof(ApplyHeightmap))]
	public class ApplyHeightmapInspector : UnityEditor.Editor
	{

		UnityEngine.Terrain mTerrain;

		public override void OnInspectorGUI()
		{
			ApplyHeightmap obj = target as ApplyHeightmap;

			obj.maxHeight = EditorGUILayout.CurveField("height curve", obj.maxHeight, GUILayout.Height(50));



			mTerrain = obj.GetComponent<Terrain>();

			if (mTerrain == null) {
				GUILayout.Label("Select a Terrain to apply the heightmap.");
			}

            GUILayout.Space(5);
            GUILayout.Label("Import from Texture", "boldLabel");
            obj.mHeightmap = EditorGUILayout.ObjectField("heightmap", obj.mHeightmap, typeof(Texture2D), false) as Texture2D;
            GUI.enabled = mTerrain != null && obj.mHeightmap != null;
            if (GUILayout.Button("Height To Terrain"))
                Apply(obj.mHeightmap, mTerrain, obj.maxHeight);

            GUILayout.Space(5);
            GUI.enabled = true;
            GUILayout.Label("Import from Text Asset", "boldLabel");
            obj.mHeightmapText = EditorGUILayout.ObjectField("heightmap text", obj.mHeightmapText, typeof(TextAsset), false) as TextAsset;
            GUI.enabled = mTerrain != null && obj.mHeightmapText != null;
            if (GUILayout.Button("Height To Terrain"))
                Apply(obj.mHeightmapText, mTerrain, obj.maxHeight);
				

		}

		static float SampleHeightMap(Color32 value, float res)
		{

			float h = ((-value.r + 256) + value.g) / (res+2);
			return h;
		}
		
		public static void Apply(TextAsset heightmap, UnityEngine.Terrain terrain, AnimationCurve curve)
		{
			float[,] array = null;
			int resolution = 0;
#pragma warning disable 219
            float sealevel;
#pragma warning restore
            using (System.IO.StreamReader file = new System.IO.StreamReader(AssetDatabase.GetAssetPath(heightmap))) {
				resolution = System.Convert.ToInt32(file.ReadLine());
				sealevel = float.Parse(file.ReadLine());
				array = new float[resolution,resolution];
				Debug.Log(resolution);
				long i = 0;
				while(file.Peek() >= 0)
				{

					float v = System.Convert.ToSingle(file.ReadLine());
					array[(resolution-1) - ( i / resolution ), i % resolution] = curve.Evaluate( v );
					i++;
				}
			}

			TerrainData data = terrain.terrainData;
			
			data.heightmapResolution = resolution;
			data.baseMapResolution = resolution;
			data.SetHeights(0,0,array);
			EditorUtility.SetDirty(data);
			EditorUtility.SetDirty(terrain);

		}

		public static void Apply(Texture2D heightmap, UnityEngine.Terrain terrain, AnimationCurve curve)
		{
			TerrainData data = terrain.terrainData;

			data.heightmapResolution = heightmap.width+1;
			data.baseMapResolution = heightmap.width;
			Color32[] pixels = heightmap.GetPixels32();
			float[,] heightmapData = new float[heightmap.width,heightmap.height];

			for(int y = 0;y < heightmapData.GetLength(1);y++)
			{
				for(int x = 0;x < heightmapData.GetLength(0);x++)
				{
					float sample = SampleHeightMap(pixels[x * heightmap.width + y], (float)data.heightmapResolution);
					heightmapData[x,y] = curve.Evaluate( sample );
				}
			}

			for (int y = 0; y < heightmapData.GetLength(1); y++) {
				for (int x = 0; x < heightmapData.GetLength(0); x++) {
					heightmapData[x,y] = GetBlurred(x,y, heightmapData);
				}
			}
			
			data.SetHeights(0, 0, heightmapData);
			EditorUtility.SetDirty(data);
			EditorUtility.SetDirty(terrain);
		}


		static float GetBlurred(int x, int y, float[,] heightmapData)
		{
			int max = heightmapData.GetLength(0) - 1;
			float f = heightmapData [x, y];
			if (x != 0 && x != max) {
				if (y != 0 && y != max) {
					float cnt = 0;
					for (int ix = x-2; ix<x+2; ix++) {
						for (int iy = y-2; iy<y+2; iy++) {
							if (x == ix && y == iy)
								continue;
							if (ix < 0 || iy < 0 || ix > max || iy > max)
								continue;
							cnt++;
							f += heightmapData [ix, iy];
						}
					}
					if (cnt > 0)
						f = ((f / cnt) + heightmapData [x, y]) * 0.5f;
					else
						f = heightmapData [x, y];
				}
			}
			return f;
		}
	}
}