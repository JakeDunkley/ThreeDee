using System.Numerics;
using ThreeDee.Structs;

namespace ThreeDee;

public static class RenderManager
{
    private static readonly float _ppWidth = 1f;
    private static readonly float _ppHeight = 1f;
    private static readonly float _ppDepth = 1f;
    public static readonly Color[,] ColorBuffer = new Color[WindowManager.WindowSizeY, WindowManager.WindowSizeX];
    public static readonly float[,] DepthBuffer = new float[WindowManager.WindowSizeY, WindowManager.WindowSizeX];

    public static List<Vector3> WorldVertexBuffer = new();
    public static List<Triangle3D> WorldTriangleBuffer = new();
    public static List<Vector2> ScreenSpaceVertexBuffer = new();
    public static List<Triangle2D> ScreenSpaceTriangleBuffer = new();
    public static List<Material> MaterialBuffer = new();

    private static void ProjectWorldVertsToScreenSpace()
    {
        ScreenSpaceVertexBuffer = new(WorldVertexBuffer.Count);

        float xRatio = WindowManager.WindowSizeX / _ppWidth;
        float yRatio = WindowManager.WindowSizeY / _ppHeight;

        float halfScreenWidth = 0.5f * WindowManager.WindowSizeX;
        float halfScreenHeight = 0.5f * WindowManager.WindowSizeY;

        foreach (Vector3 vertex in WorldVertexBuffer)
        {
            float zRatio = _ppDepth / vertex.Z;

            ScreenSpaceVertexBuffer.Add(new Vector2(
                (vertex.X * zRatio * xRatio) + halfScreenWidth,
                (vertex.Y * zRatio * yRatio) + halfScreenHeight
            ));
        }
    }

    private static void GenerateScreenSpaceTriangles()
    {
        ScreenSpaceTriangleBuffer = new(WorldTriangleBuffer.Count);

        foreach (Triangle3D tri in WorldTriangleBuffer)
        {
            ScreenSpaceTriangleBuffer.Add(new Triangle2D(
                tri.VertexBufferIndices.Item1,
                tri.VertexBufferIndices.Item2,
                tri.VertexBufferIndices.Item3
            ));
        }
    }

    public static void RenderTest()
    {
        ProjectWorldVertsToScreenSpace();
        GenerateScreenSpaceTriangles();

        foreach (Triangle2D tri in ScreenSpaceTriangleBuffer)
        {
            Color randomColor = Color.Random;

            int[] bounds = tri.CalculatePixelScreenSpaceBounds();

            for (int row = bounds[1]; row < bounds[3]; row++)
            {
                for (int col = bounds[0]; col < bounds[2]; col++)
                {
                    Vector2 ssPoint = new(col, row);

                    if (tri.IsPointInside(ssPoint))
                    {
                        WindowManager.PixelGrid[row, col] = randomColor;
                    }
                }
            }
        }
    }
}