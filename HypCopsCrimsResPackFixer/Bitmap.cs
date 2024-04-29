using BigGustave;
using System.Drawing;

namespace HypCopsCrimsResPackFixer;

class Bitmap
{
    public readonly Pixel[] Raw;
    public readonly int Width;
    public readonly int Height;
    public readonly bool HasAlphaChannel;

    public Bitmap(Stream s, bool leaveOpen = false)
    {
        Png png = Png.Open(s);
        Width = png.Width;
        Height = png.Height;
        HasAlphaChannel = png.HasAlphaChannel;
        Raw = new Pixel[Width * Height];
        for (int i = 0, y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                Raw[i++] = png.GetPixel(x, y);
        if (!leaveOpen)
            s.Close();
    }
    public Bitmap(int width, int height, bool hasAlphaChannel = true)
    {
        Width = width;
        Height = height;
        HasAlphaChannel = hasAlphaChannel;
        Raw = new Pixel[Width * Height];
    }

    public void CopyTo(Bitmap target, Rectangle rect, Point targetLT)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(rect.Width);
        ArgumentOutOfRangeException.ThrowIfNegative(rect.Height);
        ArgumentOutOfRangeException.ThrowIfNegative(rect.Left);
        ArgumentOutOfRangeException.ThrowIfNegative(rect.Top);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(rect.Right, Width);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(rect.Bottom, Height);

        ArgumentOutOfRangeException.ThrowIfNegative(targetLT.X);
        ArgumentOutOfRangeException.ThrowIfNegative(targetLT.Y);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(targetLT.X, target.Width);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(targetLT.Y, target.Height);

        for (int y = 0; y < rect.Height; y++)
        {
            int p0 = (rect.Y + y) * Width + rect.X;
            int p1 = (targetLT.Y + y) * target.Width + targetLT.X;
            Array.Copy(Raw, p0, target.Raw, p1, rect.Width);
        }
    }

    public void Save(Stream s, bool leaveOpen = false)
    {
        PngBuilder builder = PngBuilder.Create(Width, Height, HasAlphaChannel);
        for (int i = 0, y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                builder.SetPixel(Raw[i++], x, y);
        builder.Save(s);
        if (!leaveOpen)
            s.Close();
    }

    internal void Save(object value)
    {
        throw new NotImplementedException();
    }
}
