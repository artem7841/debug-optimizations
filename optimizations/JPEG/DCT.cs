using System;
using JPEG.Utilities;

namespace JPEG;

public class DCT
{
	static readonly double[] CosTable = new double[64];
	const int DCTSize = 8;
	private static readonly double[] AlphaTable = new double[8];
	
	static DCT() 
	{
		for (int i = 0; i < DCTSize; i++)
		{
			AlphaTable[i] = i == 0 ? 1 / Math.Sqrt(2) : 1.0;
		}
		
		for (int x = 0; x < DCTSize; x++)
		{
			for (int u = 0; u < DCTSize; u++)
			{
				CosTable[x*8 + u] = Math.Cos((2 * x + 1) * u * Math.PI / (2 * DCTSize));
			}
		}
	}

	public static double[,] DCT2D(double[,] input)
	{
		var tempCoeffs = new double[DCTSize, DCTSize];
		var coeffs = new double[DCTSize, DCTSize];
		
		for (int v = 0; v < DCTSize; v++)
		{
			for (int u = 0; u < DCTSize; u++)
			{
				double sum = 0;

				for (int y = 0; y < DCTSize; y++)
					sum += input[v, y] * CosTable[y*8 + u];

				tempCoeffs[v, u] = sum * AlphaTable[u];
			}
		}
		
		for (int u = 0; u < DCTSize; u++)
		{
			for (int v = 0; v < DCTSize; v++)
			{
				double sum = 0;

				for (int x = 0; x < DCTSize; x++)
					sum += tempCoeffs[x, u] * CosTable[x*8 + v];

				coeffs[u, v] = sum * AlphaTable[v];
			}
		}
		
		return coeffs;
	}

	public static void IDCT2D(double[,] coeffs, double[,] output)
	{
		
		for (var x = 0; x < DCTSize; x++)
		{
			for (var y = 0; y < DCTSize; y++)
			{
				double sum = 0;

				for (int u = 0; u < DCTSize; u++)
				{
					for (int v = 0; v < DCTSize; v++)
					{
						sum += coeffs[u, v] *
						       CosTable[x*8 + u] *
						       CosTable[y*8 + v] *
						       AlphaTable[u] * AlphaTable[v];
					}
				}

				output[x, y] = sum * Beta(DCTSize, DCTSize);
			}
		}
	}

	private static float Beta(int height, int width)
	{
		return 1f / width + 1f / height;
	}
}