using System;
namespace Terra
{
	class HeightMap
	{
		public float[,] Heights { get; set; }
		private INoiseGenerator Perlin { get; set; }
		public int Size { get; set; }
		bool useSimplex;
		public HeightMap(int size, int seed, bool simplex)
		{
			
			Size = size;
			Heights = new float[Size, Size];
			if (simplex) {
				Perlin = new Simplex();
			} else {
				Perlin = new PerlinGenerator(seed);
			}
		}


		public void AddPerlinNoise(float f, float height)
		{
			for (int i = 0; i < Size; i++)
			{
				for (int j = 0; j < Size; j++)
				{
					Heights[i, j] += (Perlin.Generate2D(f * i / (float)Size, f * j / (float)Size)) * height ;
				}
			}
		}

		public void Perturb(float f, float d)
		{
			int u, v;
			float[,] temp = new float[Size, Size];
			for (int i = 0; i < Size; ++i)
			{
				for (int j = 0; j < Size; ++j)
				{
					u = i + (int)(Perlin.Generate3D(f * i / (float)Size, f * j / (float)Size, 0) * d);
					v = j + (int)(Perlin.Generate3D(f * i / (float)Size, f * j / (float)Size, 1) * d);
					if (u < 0) u = 0; if (u >= Size) u = Size - 1;
					if (v < 0) v = 0; if (v >= Size) v = Size - 1;
					temp[i, j] = Heights[u, v];
				}
			}
			Heights = temp;
		}
		
		public void Erode(float smoothness)
		{
			for (int i = 1; i < Size - 1; i++)
			{
				for (int j = 1; j < Size - 1; j++)
				{
					float d_max = 0.0f;
					int[] match = { 0, 0 };
					
					for (int u = -1; u <= 1; u++)
					{
						for (int v = -1; v <= 1; v++)
						{
							if(Math.Abs(u) + Math.Abs(v) > 0)
							{
								float d_i = Heights[i, j] - Heights[i + u, j + v];
								if (d_i > d_max)
								{
									d_max = d_i;
									match[0] = u; match[1] = v;
								}
							}
						}
					}
					
					if(0 < d_max && d_max <= (smoothness / (float)Size))
					{
						float d_h = 0.5f * d_max;
						Heights[i, j] -= d_h;
						Heights[i + match[0], j + match[1]] += d_h;
					}
				}
			}
		}
		
		public void Smoothen()
		{
			for (int i = 1; i < Size - 1; ++i)
			{
				for (int j = 1; j < Size - 1; ++j)
				{
					float total = 0.0f;
					for (int u = -1; u <= 1; u++)
					{
						for (int v = -1; v <= 1; v++)
						{
							total += Heights[i + u, j + v];
						}
					}
					
					Heights[i, j] = total / 9.0f;
				}
			}
		}
	}
}