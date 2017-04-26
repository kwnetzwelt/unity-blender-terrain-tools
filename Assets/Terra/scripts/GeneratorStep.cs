using UnityEngine;
using System.Collections;
using System;


namespace Terra
{
	[Serializable]
	public class GeneratorStep
	{
		public enum StepType
		{
			addNoise,
			perturb,
			erode,
			smooth
		}
		public override string ToString()
		{
			return string.Format("[GeneratorStep]");
		}

		public StepType type = StepType.addNoise;
		public float noiseSize = 5;
		public float noiseHeight = 5;

		public Vector2 perturbVector = new Vector2(1,1);
		public float erodeIntensity = 5;
		public int erodeIterations = 1;
	}
}