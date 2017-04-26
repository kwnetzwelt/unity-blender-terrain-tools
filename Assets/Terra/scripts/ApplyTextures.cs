using UnityEngine;
using System.Collections;

namespace Terra
{
	[RequireComponent(typeof(Terrain))]
	public class ApplyTextures : MonoBehaviour
	{
		public AnimationCurve steepnessCurve = new AnimationCurve();

		public Texture2D lowTexture;
		public Texture2D medTexture;
		public Texture2D higTexture;

		public Texture2D steepLowTexture;
		public Texture2D steepMedTexture;
		public Texture2D steepHigTexture;

		public Texture2D variation1;
		public Texture2D variation2;

		public float variationInfluence = 0.2f;

		public float lowToMedValue = 0.33f;

		public float medToHighValue = 0.66f;

	}

}