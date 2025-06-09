using System.Numerics;

namespace ThreeDee.Structs;

public struct Triangle3D(int a, int b, int c)
{
    public (int, int, int) VertexBufferIndices = (a, b, c);

    // public readonly Triangle3D ProjectOntoProjectionPlane(float depth)
    // {
    //     float[] zRatios = [depth / Vertices[0].Z, depth / Vertices[1].Z, depth / Vertices[2].Z];

    //     Vector3 aPrime = new(Vertices[0].X * zRatios[0], Vertices[0].Y * zRatios[0], 1f);
    //     Vector3 bPrime = new(Vertices[1].X * zRatios[1], Vertices[1].Y * zRatios[1], 1f);
    //     Vector3 cPrime = new(Vertices[2].X * zRatios[2], Vertices[2].Y * zRatios[2], 1f);

    //     return new Triangle3D(aPrime, bPrime, cPrime);
    // }

    // public readonly Triangle2D ProjectToScreenSpace(float projectionPlaneDepth, float ppWidth, float ppHeight, float screenWidth, float screenHeight)
    // {
    //     float widthRatio = screenWidth / ppWidth;
    //     float heightRatio = screenHeight / ppHeight;

    //     Triangle3D ppt = ProjectOntoProjectionPlane(projectionPlaneDepth);
    //     Vector2 aPrime = new((ppt.Vertices[0].X * widthRatio) + (0.5f * screenWidth), (ppt.Vertices[0].Y * heightRatio) + (0.5f * screenHeight));
    //     Vector2 bPrime = new((ppt.Vertices[1].X * widthRatio) + (0.5f * screenWidth), (ppt.Vertices[1].Y * heightRatio) + (0.5f * screenHeight));
    //     Vector2 cPrime = new((ppt.Vertices[2].X * widthRatio) + (0.5f * screenWidth), (ppt.Vertices[2].Y * heightRatio) + (0.5f * screenHeight));

    //     return new Triangle2D(aPrime, bPrime, cPrime);
    // }
}

public struct Triangle2D(int a, int b, int c)
{
    public (int, int, int) VertexBufferIndices = (a, b, c);

    private static bool IsPointToRightOfLine(Vector2 a, Vector2 b, Vector2 point)
    {
        Vector2 aToB = Vector2.Normalize(Vector2.Subtract(b, a));
        Vector2 aToPoint = Vector2.Normalize(Vector2.Subtract(point, a));

        Vector2 abNormal = new(aToB.Y, -aToB.X);

        return Vector2.Dot(abNormal, aToPoint) >= 0f;
    }

    public readonly bool IsPointInside(Vector2 point)
    {
        bool ab = IsPointToRightOfLine(RenderManager.ScreenSpaceVertexBuffer[VertexBufferIndices.Item1], RenderManager.ScreenSpaceVertexBuffer[VertexBufferIndices.Item2], point);
        bool bc = IsPointToRightOfLine(RenderManager.ScreenSpaceVertexBuffer[VertexBufferIndices.Item2], RenderManager.ScreenSpaceVertexBuffer[VertexBufferIndices.Item3], point);
        bool ca = IsPointToRightOfLine(RenderManager.ScreenSpaceVertexBuffer[VertexBufferIndices.Item3], RenderManager.ScreenSpaceVertexBuffer[VertexBufferIndices.Item1], point);

        return !(ab || bc || ca);
    }

    public readonly int[] CalculatePixelScreenSpaceBounds()
    {
        Vector2 min = Vector2.Min(
            Vector2.Min(
                RenderManager.ScreenSpaceVertexBuffer[VertexBufferIndices.Item1],
                RenderManager.ScreenSpaceVertexBuffer[VertexBufferIndices.Item2]
            ),
            RenderManager.ScreenSpaceVertexBuffer[VertexBufferIndices.Item3]
        );

        Vector2 max = Vector2.Max(
            Vector2.Max(
                RenderManager.ScreenSpaceVertexBuffer[VertexBufferIndices.Item1],
                RenderManager.ScreenSpaceVertexBuffer[VertexBufferIndices.Item2]
            ),
            RenderManager.ScreenSpaceVertexBuffer[VertexBufferIndices.Item3]
        );

        return
        [
            Math.Max(0, (int)Math.Floor(min.X)),
            Math.Max(0, (int)Math.Floor(min.Y)),
            Math.Min(WindowManager.WindowSizeX - 1, (int)Math.Ceiling(max.X)),
            Math.Min(WindowManager.WindowSizeY - 1, (int)Math.Ceiling(max.Y))
        ];
    }
}