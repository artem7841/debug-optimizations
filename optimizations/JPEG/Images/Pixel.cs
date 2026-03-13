using System;
using System.Linq;

namespace JPEG.Images;

public struct Pixel
{
	
	public float R;
	public float G;
	public float B;

	public float Y;
	public float Cb;
	public float Cr;

	public Pixel(float firstComponent, float secondComponent, float thirdComponent, byte pixelFormat)
	{
		if (pixelFormat != 1 && pixelFormat != 0)
			throw new FormatException("Unknown pixel format: " + pixelFormat);
		if (pixelFormat == 0)
		{
			R = firstComponent;
			G = secondComponent;
			B = thirdComponent;

			Y = 16.0f + (65.738f * R + 129.057f * G + 24.064f * B) / 256.0f;
			Cb = 128.0f + (-37.945f * R - 74.494f * G + 112.439f * B) / 256.0f;
			Cr = 128.0f + (112.439f * R - 94.154f * G - 18.285f * B) / 256.0f;
		}

		if (pixelFormat == 1)
		{
			Y = firstComponent;
			Cb = secondComponent;
			Cr = thirdComponent;
			
			R = (298.082f * Y + 408.583f * Cr) / 256.0f - 222.921f;
			G = (298.082f * Y - 100.291f * Cb - 208.120f * Cr) / 256.0f + 135.576f;
			B = (298.082f * Y + 516.412f * Cb) / 256.0f - 276.836f;
		}
	}
}