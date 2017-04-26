using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace TerrainFromMesh
{

    [AddComponentMenu("Terrain/Terrain From Mesh")]
    [RequireComponent(typeof(Terrain))]
    public class TerrainFromMeshComponent : MonoBehaviour
    {
	    public Transform TerrainParent;
	    [HideInInspector]
	    [NonSerialized]
	    public Bounds mBounds;
	    public float mResolution;
	    public Queue<Ray> mRays = new Queue<Ray>();
	    void OnDrawGizmos()
	    {
		    Gizmos.DrawWireCube(mBounds.center, mBounds.size);
		    foreach (var r in mRays) {
			    Gizmos.DrawRay(r.origin, r.direction * 50);
		    }

	    }
    }

}
