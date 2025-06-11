using System.Numerics;

namespace ThreeDee.Structs;

public struct Color
{
    private static readonly Random _rng = new();

    public Vector3 RGB;

    public readonly float R => RGB.X;
    public readonly float G => RGB.Y;
    public readonly float B => RGB.Z;

    public readonly byte ByteR => (byte)(255 * R);
    public readonly byte ByteG => (byte)(255 * G);
    public readonly byte ByteB => (byte)(255 * B);

    public Color(float r, float g, float b)
    {
        RGB = new(r, g, b);
    }

    public Color(int r, int g, int b)
    {
        RGB = new(r, g, b);
    }

    public Color(double r, double g, double b)
    {
        RGB = new((float)r, (float)g, (float)b);
    }

    public Color(float l)
    {
        RGB = new(l, l, l);
    }

    public static Color operator *(float scalar, Color color)
    {
        return new Color(scalar * color.R, scalar * color.G, scalar * color.B);
    }

    public static Color Random => new(_rng.NextSingle(), _rng.NextSingle(), _rng.NextSingle());
    public static Color Black => new(0, 0, 0);
    public static Color White => new(1, 1, 1);
    public static Color Red => new(1, 0, 0);
    public static Color Green => new(0, 1, 0);
    public static Color Blue => new(0, 0, 1);
}