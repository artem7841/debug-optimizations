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
		int width = bmp.Width - bmp.Width % 8;
		int height = bmp.Height - bmp.Height % 8;

		var matrix = new Matrix(height, width);

		using var clone = bmp.Clone(
			new Rectangle(0,0,width,height),
			System.Drawing.Imaging.PixelFormat.Format24bppRgb);

		BitmapData data = clone.LockBits(
			new Rectangle(0,0,width,height),
			ImageLockMode.ReadOnly,
			System.Drawing.Imaging.PixelFormat.Format24bppRgb);

		try
		{
			int stride = data.Stride;

			unsafe
			{
				byte* scan0 = (byte*)data.Scan0;

				Parallel.For(0, height, y =>
				{
					byte* row = scan0 + y * stride;

					for (int x = 0; x < width; x++)
					{
						byte* pixel = row + x * 3;

						matrix.Pixels[y,x] = new Pixel(
							pixel[2], 
							pixel[1], 
							pixel[0], 
							0);
					}
				});
			}
		}
		finally
		{
			clone.UnlockBits(data);
		}

		return matrix;
	}

	public static explicit operator Bitmap(Matrix matrix)
	{
		var bmp = new Bitmap(matrix.Width, matrix.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
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