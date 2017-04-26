using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Terra;

[Serializable]
public class TerraGeneratorSettings
{
	public int seed = 1;
    public bool simplex;
	System.Random rnd;
	public int terraSize = 1024;
	public int height = 200;
	Terrain terrain;

	public GeneratorStep[] mSteps = new GeneratorStep[0];

	public TerraGeneratorSettings Copy()
	{
		return this.MemberwiseClone() as TerraGeneratorSettings;
	}
}

