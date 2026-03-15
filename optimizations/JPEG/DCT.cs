using System;
using JPEG.Utilities;

namespace JPEG;

public class DCT
{
    static readonly double[,] CosTable = new double[8, 8];
    private static int DCTSize = 8;
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
                CosTable[x, u] = Math.Cos((2 * x + 1) * u * Math.PI / (2 * DCTSize));
            }
        }
    }

    public static double[,] DCT2D(double[,] input)
    {
        var height = input.GetLength(0); 
        var width = input.GetLength(1);
        var coeffs = new double[DCTSize, DCTSize];

        for (int u = 0; u < DCTSize; u++)
        {
            for (int v = 0; v < DCTSize; v++)
            {
                double sum = 0;

                for (int x = 0; x < DCTSize; x++)
                {
                    for (int y = 0; y < DCTSize; y++)
                    {
                        sum += input[x, y] *
                               CosTable[x, u] *
                               CosTable[y, v];
                    }
                }
				
                coeffs[u, v] = sum * Beta(height, width) * AlphaTable[u] * AlphaTable[v];
            }
        }
		
        return coeffs;
    }

    public static void IDCT2D(double[,] coeffs, double[,] output)
    {
        for (var x = 0; x < coeffs.GetLength(1); x++)
        {
            for (var y = 0; y < coeffs.GetLength(0); y++)
            {
                double sum = 0;

                for (int u = 0; u < DCTSize; u++)
                {
                    for (int v = 0; v < DCTSize; v++)
                    {
                        sum += coeffs[u, v] *
                               CosTable[x, u] *
                               CosTable[y, v] *
                               AlphaTable[u] * AlphaTable[v];
                    }
                }

                output[x, y] = sum * Beta(coeffs.GetLength(0), coeffs.GetLength(1));
            }
        }
    }

    private static float Beta(int height, int width)
    {
        return 1f / width + 1f / height;
    }
}