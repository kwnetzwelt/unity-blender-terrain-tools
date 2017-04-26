using UnityEngine;
using System.Collections;

namespace Terra
{
	[RequireComponent(typeof(Terrain))]
	public class ApplyHeightmap : MonoBehaviour
	{
		public Texture2D mHeightmap;
		public TextAsset mHeightmapText;
		public AnimationCurve maxHeight = new AnimationCurve();

	}

}