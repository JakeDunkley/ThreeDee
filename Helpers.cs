using System.Text;

namespace ThreeDee;

public static class Helpers
{
    /// <summary>
    /// Generates a bitmap file from a 2D list of color data & writes it to disk.
    /// </summary>
    /// <remarks>
    /// Credit to Sebastian Lague's video "Coding Adventure: Software Rasterizer"
    /// https://www.youtube.com/watch?v=yyJ-hdISgnw
    /// </remarks>
    /// <param name="colorData"></param>
    /// <param name="fileName"></param>
    public static void WriteImageDataToFile(List<List<System.Numerics.Vector3>> colorData, string fileName)
    {
        FileStream fileStream = new($"{fileName}.bmp", FileMode.Create);
        using BinaryWriter writer = new(fileStream);

        uint[] fileSectionSizes = [14, 40, (uint)(colorData.Count * 4)];

        // Bitmap Headers
        writer.Write(Encoding.ASCII.GetBytes("BM"));
        writer.Write(fileSectionSizes[0] + fileSectionSizes[1] + fileSectionSizes[2]);
        writer.Write((uint)0);
        writer.Write(fileSectionSizes[0] + fileSectionSizes[1]);
        writer.Write(fileSectionSizes[1]);
        writer.Write((uint)colorData.Count);
        writer.Write((uint)colorData[0].Count);
        writer.Write((ushort)1);
        writer.Write((ushort)(8 * 4));
        writer.Write((uint)0);
        writer.Write(fileSectionSizes[2]);
        writer.Write(new byte[16]);

        // Actual color data
        foreach (List<System.Numerics.Vector3> row in colorData)
        {
            foreach (System.Numerics.Vector3 color in row)
            {
                writer.Write((byte)(color.Z * 255));
                writer.Write((byte)(color.Y * 255));
                writer.Write((byte)(color.X * 255));
                writer.Write((byte)0);
            }
        }
    }

    public static long TimeThis<T>(Action<T> action, T arg)
    {
        SFML.System.Clock clock = new();
        clock.Restart();
        action(arg);
        return clock.ElapsedTime.AsMicroseconds();
    } 
}