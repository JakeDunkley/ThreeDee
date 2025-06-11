using System.Numerics;

namespace ThreeDee.Structs;

public struct Triangle3D(int a, int b, int c)
{
    public (int, int, int) VertexBufferIndices = (a, b, c);

    private static bool IsPointToRightOfLine(Vector3 a, Vector3 b, Vector3 point)
    {
        Vector3 aToB = Vector3.Normalize(Vector3.Subtract(b, a));
        Vector3 aToPoint = Vector3.Normalize(Vector3.Subtract(point, a));

        Vector3 abNormal = new(aToB.Y, -aToB.X, 0);

        return Vector3.Dot(abNormal, aToPoint) > 0f;
    }

    public readonly bool IsPointInside(Vector3 point)
    {
        bool ab = IsPointToRightOfLine(RenderManager.ScreenSpaceVertexBuffer[VertexBufferIndices.Item1], RenderManager.ScreenSpaceVertexBuffer[VertexBufferIndices.Item2], point);
        bool bc = IsPointToRightOfLine(RenderManager.ScreenSpaceVertexBuffer[VertexBufferIndices.Item2], RenderManager.ScreenSpaceVertexBuffer[VertexBufferIndices.Item3], point);
        bool ca = IsPointToRightOfLine(RenderManager.ScreenSpaceVertexBuffer[VertexBufferIndices.Item3], RenderManager.ScreenSpaceVertexBuffer[VertexBufferIndices.Item1], point);

        return !(ab || bc || ca);
    }

    public static float CalculateArea(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 ca = Vector3.Subtract(a, c);
        Vector3 rotCA = new(ca.Y, -ca.X, 0);

        Vector3 cb = Vector3.Subtract(b, c);

        float projCoef = Vector3.Dot(rotCA, cb) / Vector3.Dot(rotCA, rotCA);
        Vector2 proj = new(projCoef * rotCA.X, projCoef * rotCA.Y);

        return 0.5f * ca.Length() * proj.Length();
    }

    public readonly float CalculateDepthAt(Vector3 point)
    {
        float abp = CalculateArea(RenderManager.ScreenSpaceVertexBuffer[VertexBufferIndices.Item1], RenderManager.ScreenSpaceVertexBuffer[VertexBufferIndices.Item2], point);
        float bcp = CalculateArea(RenderManager.ScreenSpaceVertexBuffer[VertexBufferIndices.Item2], RenderManager.ScreenSpaceVertexBuffer[VertexBufferIndices.Item3], point);
        float cap = CalculateArea(RenderManager.ScreenSpaceVertexBuffer[VertexBufferIndices.Item3], RenderManager.ScreenSpaceVertexBuffer[VertexBufferIndices.Item1], point);

        float sum = abp + bcp + cap;

        return ((bcp * RenderManager.WorldVertexBuffer[VertexBufferIndices.Item1].Z)
            + (cap * RenderManager.WorldVertexBuffer[VertexBufferIndices.Item2].Z)
            + (abp * RenderManager.WorldVertexBuffer[VertexBufferIndices.Item3].Z))
            / sum;
    }

    public readonly int[] CalculatePixelScreenSpaceBounds()
    {
        Vector3 min = Vector3.Min(
            Vector3.Min(
                RenderManager.ScreenSpaceVertexBuffer[VertexBufferIndices.Item1],
                RenderManager.ScreenSpaceVertexBuffer[VertexBufferIndices.Item2]
            ),
            RenderManager.ScreenSpaceVertexBuffer[VertexBufferIndices.Item3]
        );

        Vector3 max = Vector3.Max(
            Vector3.Max(
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