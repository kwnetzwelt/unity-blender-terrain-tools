using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace Terra.Editor
{
	[CustomEditor(typeof(ApplyTextures))]
	public class ApplyTexturesInspector : UnityEditor.Editor
	{

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (GUILayout.Button("Apply")) {
				Apply(target as ApplyTextures);
			}

		}

		void PrepareTextures(ApplyTextures target, TerrainData terrainData)
		{
			List<SplatPrototype> list = new List<SplatPrototype>();

			SplatPrototype sp = new SplatPrototype();
			sp.texture = target.steepLowTexture;
			sp.tileSize = Vector2.one * 15;
			list.Add(sp);

			sp = new SplatPrototype();
			sp.texture = target.lowTexture;
			sp.tileSize = Vector2.one * 15;
			list.Add(sp);



			sp = new SplatPrototype();
			sp.texture = target.steepMedTexture;
			sp.tileSize = Vector2.one * 15;
			list.Add(sp);

			sp = new SplatPrototype();
			sp.texture = target.medTexture;
			sp.tileSize = Vector2.one * 15;
			list.Add(sp);



			sp = new SplatPrototype();
			sp.texture = target.steepHigTexture;
			sp.tileSize = Vector2.one * 15;
			list.Add(sp);

			sp = new SplatPrototype();
			sp.texture = target.higTexture;
			sp.tileSize = Vector2.one * 15;
			list.Add(sp);

			sp = new SplatPrototype();
			sp.texture = target.variation1;
			sp.tileSize = Vector2.one * 15;
			list.Add(sp);

			terrainData.splatPrototypes = list.ToArray();

		}

		void Apply(ApplyTextures target)
		{

			Terrain terra = target.GetComponent<Terrain>();
			float steepness = 0;
			float height = 0;

			PrepareTextures(target, terra.terrainData);

			float[,,] map = new float[terra.terrainData.alphamapWidth, terra.terrainData.alphamapHeight, 7];
			float[,] heights = terra.terrainData.GetHeights(0, 0, terra.terrainData.heightmapResolution, terra.terrainData.heightmapResolution);
			float maxHeight = 0;
			float minHeight = float.MaxValue;
			for (int y = 0; y < heights.GetLength(0); y++) {
				for (int x = 0; x < heights.GetLength(1); x++) {
					maxHeight = Mathf.Max(maxHeight, heights[x,y]);
					minHeight = Mathf.Min(minHeight, heights[x,y]);
				}
			}

			for (int y = 0; y < terra.terrainData.alphamapWidth; y++) {
				for (int x = 0; x < terra.terrainData.alphamapWidth; x++) {
					float normX = x * 1.0f / (terra.terrainData.alphamapWidth - 1);
					float normY = y * 1.0f / (terra.terrainData.alphamapHeight - 1);


					steepness = terra.terrainData.GetSteepness(normY,normX);
					height = heights[(int)(normX * (terra.terrainData.heightmapResolution-1)) ,(int)(normY * (terra.terrainData.heightmapResolution-1))];
					height = (height - minHeight) / (maxHeight-minHeight);
					float steepfrag = Mathf.Clamp01( target.steepnessCurve.Evaluate( steepness / 90.0f ) );

                    //float lowFrag = Mathf.Clamp01(height / target.lowToMedValue);
#pragma warning disable 219
                    float medFrag = (height-target.lowToMedValue) / (target.medToHighValue - target.lowToMedValue);
					float higFrag = (height-target.medToHighValue) / (1-target.medToHighValue);
#pragma warning restore
                    // variation
                    map[x, y, 6] =
						target.variationInfluence *
						target.variation2.GetPixel(x % target.variation2.width ,y % target.variation2.height).r;



					map[x, y, 1] = (1-steepfrag)* Mathf.Clamp01(1-(height-target.lowToMedValue+0.1f) / (0.1f));
					map[x, y, 3] = (1-steepfrag)* Mathf.Clamp01(  (height-target.lowToMedValue+0.1f) /(0.1f));
					map[x, y, 5] = (1-steepfrag)* Mathf.Clamp01(  (height-target.medToHighValue+0.1f) /(0.1f));

					if(height > target.lowToMedValue)
					{
						map[x, y, 3] = (1-steepfrag)* Mathf.Clamp01(1-(height-target.medToHighValue+0.1f) /(0.1f));
					}




					map[x, y, 0] = steepfrag * Mathf.Clamp01(1-(height-target.lowToMedValue+0.1f) / (0.1f));
					map[x, y, 2] = steepfrag * Mathf.Clamp01(  (height-target.lowToMedValue+0.1f) /(0.1f));
					map[x, y, 4] = steepfrag * Mathf.Clamp01(  (height-target.medToHighValue+0.1f) /(0.1f));
					if(height>target.lowToMedValue)
					{
						map[x, y, 2] = steepfrag * Mathf.Clamp01(1-(height-target.medToHighValue+0.1f) /(0.1f));
					}

				}
			}
			terra.terrainData.SetAlphamaps(0, 0, map);
		}

	}

}