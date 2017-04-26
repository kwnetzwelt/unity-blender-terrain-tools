using UnityEngine;
using System.Collections;
using UnityEditor;

namespace TerrainFromMesh
{
    [CustomEditor(typeof(TerrainFromMeshComponent))]
    public class TerrainFromMeshInspector : Editor
    {
        void OnEnable()
        {
            EditorApplication.update += Update;
        }

        void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        void Update()
        {
            if (generator != null)
            {
                if (!generator.MoveNext())
                    generator = null;
            }
        }

        IEnumerator generator;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (generator == null)
            {
                if (GUILayout.Button("Generate from Mesh"))
                {
                    generator = Generate();
                }
            }
            else
            {
                if (GUILayout.Button("Cancel"))
                {
                    generator = null;
                }
            }

        }


        IEnumerator Generate()
        {
            TerrainFromMeshComponent tfm = target as TerrainFromMeshComponent;

            Collider[] colliders = tfm.TerrainParent.GetComponentsInChildren<Collider>();

            if (colliders.Length <= 0)
            {
                EditorUtility.DisplayDialog("No Colliders found", "The Terrain parent object does not contain any colliders. Please enable Collider generation upon import of the mesh. ", "Dismiss");
                Debug.LogError("No colliders found in terrain parent. Aborting. ");
                yield break;
            }

            tfm.mResolution = (float)Mathf.NextPowerOfTwo((int)tfm.mResolution);
            tfm.mBounds = colliders[0].bounds;
            Terrain terrain = tfm.GetComponent<Terrain>();

            foreach (var c in colliders)
            {
                tfm.mBounds.Encapsulate(c.bounds);

                yield return 0;
            }

            tfm.transform.position = tfm.mBounds.min;

            if (terrain.terrainData == null)
            {
                if (EditorUtility.DisplayDialog("No Terrain Data found", "The target Terrain does not contain Terrain Data. ", "Create New", "Abort"))
                {
                    var terrainData = new TerrainData();
                    var terrainDataPath = AssetDatabase.GenerateUniqueAssetPath("Assets/Terrain Data.asset");
                    AssetDatabase.CreateAsset(terrainData, terrainDataPath);
                    terrain.terrainData = terrainData;
                }
                else
                {
                    Debug.LogError("No Terrain Data found. Aborting. ");
                    yield break;
                }
            }

            terrain.terrainData.heightmapResolution = (int)tfm.mResolution;
            terrain.terrainData.size = tfm.mBounds.size;
            RaycastHit hitInfo;
            float[,] data = new float[1, (int)tfm.mResolution];
            for (int y = 0; y < tfm.mResolution; y++)
            {
                for (int x = 0; x < tfm.mResolution; x++)
                {
                    Ray r = GetRay(x, y, tfm);
                    tfm.mRays.Enqueue(r);
                    if (Physics.Raycast(r, out hitInfo))
                    {
                        data[0, x] = 1 - ((hitInfo.distance - 50) / tfm.mBounds.size.y);
                    }
                }
                terrain.terrainData.SetHeights(0, y, data);

                yield return 0;
                tfm.mRays.Clear();
            }

        }

        Ray GetRay(int x, int y, TerrainFromMeshComponent pTarget)
        {
            Vector3 source = pTarget.mBounds.min;
            source += new Vector3(pTarget.mBounds.size.x * x / pTarget.mResolution, 0, 0);
            source += new Vector3(0, 0, pTarget.mBounds.size.z * y / pTarget.mResolution);
            source.y = pTarget.mBounds.max.y + 50;
            return new Ray(source, Vector3.down);
        }

    }

}