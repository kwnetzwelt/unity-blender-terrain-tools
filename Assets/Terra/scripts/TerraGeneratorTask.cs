using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Terra;

public class TerraGeneratorTask
{
	TerraGeneratorSettings settings;
	HeightMap hm;

	public TerraGeneratorTask(TerraGeneratorSettings pSettings)
	{
		settings = pSettings;
	}
	public float progress {
		get;
		private set;
	}

	public bool done
	{
		get;
		private set;
	}

	public void Run()
	{
		System.Threading.ThreadPool.QueueUserWorkItem(delegate {
			var ien = Generate();
			while(ien.MoveNext())
			{
				progress = ien.Current;
			}
			progress = 1;
			done = true;
		}, null);

	}

	IEnumerator<float> Generate()
	{
		float step = 0;
		float maxStep = 2;
		foreach (var genStep in settings.mSteps) {
			maxStep = maxStep+1;
			if(genStep.type == GeneratorStep.StepType.erode)
				maxStep+=genStep.erodeIterations;
		}

		//rnd.NextBytes(Noise.perm);
		settings.terraSize = Mathf.NextPowerOfTwo(settings.terraSize);
		
		hm = new HeightMap(settings.terraSize, settings.seed, settings.simplex);

		foreach(var genStep in settings.mSteps)
		{
			switch(genStep.type)
			{
				case GeneratorStep.StepType.addNoise:
					hm.AddPerlinNoise(genStep.noiseSize, genStep.noiseHeight); break;
				case GeneratorStep.StepType.erode:
					for(int i = 0;i<genStep.erodeIterations;i++)
					{
						hm.Erode(genStep.erodeIntensity);
						yield return step++ / maxStep;
					}
					break;
				case GeneratorStep.StepType.perturb:
					hm.Perturb(genStep.perturbVector.x, genStep.perturbVector.y);
					break;
				case GeneratorStep.StepType.smooth:
					hm.Smoothen();
					break;

			}
			yield return step++ / maxStep;
		}

		float max = 0;
		float min = float.MaxValue;
		
		for (int y = 0; y<settings.terraSize; y++) {
			for (int x = 0; x<settings.terraSize; x++) {
				max = Mathf.Max(hm.Heights [x, y], max);
				min = Mathf.Min(hm.Heights [x, y], min);
			}
		}
		yield return step++ / maxStep;
		for (int y = 0; y<settings.terraSize; y++) {
			for (int x = 0; x<settings.terraSize; x++) {
				hm.Heights [x, y] = (hm.Heights [x, y] - min) / (max - min);
			}
		}
		yield return step++ / maxStep;
	}
	public static void ApplyToTerrain(Terrain pTerrain, TerraGeneratorTask pTask)
	{

		pTerrain.terrainData.heightmapResolution = pTask.settings.terraSize+1;
		pTerrain.terrainData.size = new Vector3(pTask.settings.terraSize, pTask.settings.height, pTask.settings.terraSize);
		pTerrain.terrainData.SetHeights(0, 0, pTask.hm.Heights);

	}
}

