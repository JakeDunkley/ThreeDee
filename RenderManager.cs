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
    public static List<Vector3> WorldVertexTransformBuffer = new();
    public static List<Triangle3D> WorldTriangleBuffer = new();
    public static Vector3[] ScreenSpaceVertexBuffer = [];
    public static Triangle3D[] ScreenSpaceTriangleBuffer = [];
    public static List<Material> MaterialBuffer = new();

    private static void ProjectWorldVertsToScreenSpace()
    {
        ScreenSpaceVertexBuffer = new Vector3[WorldVertexBuffer.Count];

        float xRatio = WindowManager.RenderSizeX / _ppWidth;
        float yRatio = WindowManager.RenderSizeY / _ppHeight;

        float halfScreenWidth = 0.5f * WindowManager.RenderSizeX;
        float halfScreenHeight = 0.5f * WindowManager.RenderSizeY;

        Parallel.For(0, ScreenSpaceVertexBuffer.Length, i =>
        {
            float zRatio = _ppDepth / WorldVertexBuffer[i].Z;

            ScreenSpaceVertexBuffer[i] = new Vector3(
                (WorldVertexBuffer[i].X * zRatio * xRatio) + halfScreenWidth,
                (WorldVertexBuffer[i].Y * zRatio * yRatio) + halfScreenHeight,
                WorldVertexBuffer[i].Z
            );
        });
    }

    private static void GenerateScreenSpaceTriangles()
    {
        ScreenSpaceTriangleBuffer = new Triangle3D[WorldTriangleBuffer.Count];

        Parallel.For(0, ScreenSpaceTriangleBuffer.Length, i =>
        {
            ScreenSpaceTriangleBuffer[i] = new Triangle3D(
                WorldTriangleBuffer[i].VertexBufferIndices.Item1,
                WorldTriangleBuffer[i].VertexBufferIndices.Item2,
                WorldTriangleBuffer[i].VertexBufferIndices.Item3
            );
        });
    }

    private static void ClearDepthBuffer()
    {
        Parallel.For(0, DepthBuffer.GetLength(0), row =>
        {
            for (int col = 0; col < DepthBuffer.GetLength(1); col++)
            {
                DepthBuffer[row, col] = 0f;
            }
        });
    }

    public static void MaterialTest()
    {
        foreach (Triangle3D _ in WorldTriangleBuffer)
        {
            MaterialBuffer.Add(new Material());
        }
    }

    public static void RenderTest()
    {
        ClearDepthBuffer();
        ProjectWorldVertsToScreenSpace();
        GenerateScreenSpaceTriangles();

        for (int i = 0; i < ScreenSpaceTriangleBuffer.Length; i++)
        {
            int[] bounds = ScreenSpaceTriangleBuffer[i].CalculatePixelScreenSpaceBounds();

            Parallel.For(bounds[1], bounds[3], row =>
            {
                for (int col = bounds[0]; col < bounds[2]; col++)
                {
                    Vector3 ssPoint = new(col, row, 0);

                    // float depth = ScreenSpaceTriangleBuffer[i].CalculateDepth(ssPoint);

                    // WindowManager.PixelGrid[row, col] = new((depth + 1f) / 5f);

                    // if (depth >= 0f && depth < DepthBuffer[row, col])
                    // {
                    //     DepthBuffer[row, col] = depth;
                    //     WindowManager.PixelGrid[row, col] = Color.Green;

                    //     // float dCoef = (depth - 1.5f) / 3f;
                    //     // WindowManager.PixelGrid[row, col] = (1f - dCoef) * MaterialBuffer[i].CalculateColorAt(ssPoint);
                    // }

                    if (ScreenSpaceTriangleBuffer[i].IsPointInside(ssPoint))
                    {
                        float depthValue = ScreenSpaceTriangleBuffer[i].CalculateDepthAt(ssPoint);

                        if (DepthBuffer[row, col] == 0f || DepthBuffer[row, col] > depthValue)
                        {
                            DepthBuffer[row, col] = depthValue;
                            float dCoef = (depthValue - 3.5f) / 3f;
                            WindowManager.RawPixelGrid[row, col] = (1f - dCoef) * MaterialBuffer[i].CalculateColorAt(ssPoint);
                        }
                    }
                }
            });
        }
    }
}