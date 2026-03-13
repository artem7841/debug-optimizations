using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace JPEG.Images;

class Matrix
{
	public readonly Pixel[,] Pixels;
	public readonly int Height;
	public readonly int Width;

	public Matrix(int height, int width)
	{
		Height = height;
		Width = width;

		Pixels = new Pixel[height, width];
		for (var i = 0; i < height; ++i)
		for (var j = 0; j < width; ++j)
			Pixels[i, j] = new Pixel(0, 0, 0, 0);
	}

	public static explicit operator Matrix(Bitmap bmp)
	{
		var width = bmp.Width;
		var height = bmp.Height;
		var matrix = new Matrix(height, width);

		BitmapData bmpData = bmp.LockBits(
			new Rectangle(0, 0, width, height),
			ImageLockMode.ReadOnly,
			bmp.PixelFormat);
    
		try
		{
			int stride = bmpData.Stride;
			unsafe
			{
				byte* scan0 = (byte*)bmpData.Scan0;
            
				Parallel.For(0, height, j =>
				{
					byte* row = scan0 + j * stride;
                
					for (int i = 0; i < width; i++)
					{
						byte b = row[i * 3];
						byte g = row[i * 3 + 1];
						byte r = row[i * 3 + 2];
						matrix.Pixels[j, i] = new Pixel(r, g, b, 0);
					}
				});
			}
		}
		finally
		{
			bmp.UnlockBits(bmpData);
		}
    
		return matrix;
	}

	public static explicit operator Bitmap(Matrix matrix)
	{
		var bmp = new Bitmap(matrix.Width, matrix.Height);
		var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
		var data = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);
		int stride = data.Stride;

		unsafe
		{
			byte* ptr = (byte*)data.Scan0;

			for (int y = 0; y < matrix.Height; y++)
			{
				byte* row = ptr + y * stride;

				for (int x = 0; x < matrix.Width; x++)
				{
					var pixel = matrix.Pixels[y, x];

					row[x * 3 + 0] = (byte)pixel.B;
					row[x * 3 + 1] = (byte)pixel.G;
					row[x * 3 + 2] = (byte)pixel.R;
				}
			}
		}
		bmp.UnlockBits(data);
		return bmp;
	}

	public static int ToByte(double d)
	{
		var val = (int)d;
		if (val > byte.MaxValue)
			return byte.MaxValue;
		if (val < byte.MinValue)
			return byte.MinValue;
		return val;
	}
}