using System.Numerics;

namespace ThreeDee.Structs;

public struct Triangle
{
    public Vector3[] Vertices;

    public Triangle()
    {
        Vertices = [new Vector3(0f), new Vector3(0f), new Vector3(0f)];
    }

    public Triangle(Vector3 a, Vector3 b, Vector3 c)
    {
        Vertices = [a, b, c];
    }

    public readonly Vector3[] BoundingBox => CalculateBoundingBox();

    public static bool IsPointToRightOfLine(Vector3 a, Vector3 b, Vector3 point)
    {
        Vector3 aToB = Vector3.Normalize(Vector3.Subtract(b, a));
        Vector3 aToPoint = Vector3.Normalize(Vector3.Subtract(point, a));

        Vector3 abNormal = new(aToB.Y, -aToB.X, 0f);

        return Vector3.Dot(abNormal, aToPoint) > 0f;
    }

    public readonly bool IsPointInside(Vector3 point)
    {
        bool ab = IsPointToRightOfLine(Vertices[0], Vertices[1], point);
        bool bc = IsPointToRightOfLine(Vertices[1], Vertices[2], point);
        bool ca = IsPointToRightOfLine(Vertices[2], Vertices[0], point);

        return ab && bc && ca;
    }

    private readonly Vector3[] CalculateBoundingBox()
    {
        Vector3 min = Vector3.Min(Vector3.Min(Vertices[0], Vertices[1]), Vertices[2]);
        Vector3 max = Vector3.Max(Vector3.Max(Vertices[0], Vertices[1]), Vertices[2]);

        return
        [
            min,
            new Vector3(min.X, max.Y, min.Z),
            new Vector3(max.X, max.Y, min.Z),
            new Vector3(max.X, min.Y, min.Z),
            new Vector3(min.X, min.Y, max.Z),
            max,
            new Vector3(max.X, min.Y, max.Z)
        ];
    }
}