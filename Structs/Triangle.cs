using System.Numerics;
using SFML.System;

namespace ThreeDee.Structs;

public struct Triangle3D
{
    public Vector3[] Vertices;

    public Triangle3D()
    {
        Vertices = [new Vector3(0f), new Vector3(0f), new Vector3(0f)];
    }

    public Triangle3D(Vector3 a, Vector3 b, Vector3 c)
    {
        Vertices = [a, b, c];
    }

    public readonly Vector3[] WorldSpaceBoundingBox => CalculateBoundingBox3D();

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

    public readonly Triangle3D ProjectOntoProjectionPlane(float depth)
    {
        float[] zRatios = [depth / Vertices[0].Z, depth / Vertices[1].Z, depth / Vertices[2].Z];

        Vector3 aPrime = new(Vertices[0].X * zRatios[0], Vertices[0].Y * zRatios[0], 1f);
        Vector3 bPrime = new(Vertices[1].X * zRatios[1], Vertices[1].Y * zRatios[1], 1f);
        Vector3 cPrime = new(Vertices[2].X * zRatios[2], Vertices[2].Y * zRatios[2], 1f);

        return new Triangle3D(aPrime, bPrime, cPrime);
    }

    public readonly Triangle2D ProjectToScreenSpace(float projectionPlaneDepth, float ppWidth, float ppHeight, float screenWidth, float screenHeight)
    {
        float widthRatio = screenWidth / ppWidth;
        float heightRatio = screenHeight / ppHeight;

        Triangle3D ppt = ProjectOntoProjectionPlane(projectionPlaneDepth);
        Vector2 aPrime = new((ppt.Vertices[0].X * widthRatio) + (0.5f * screenWidth), (ppt.Vertices[0].Y * heightRatio) + (0.5f * screenHeight));
        Vector2 bPrime = new((ppt.Vertices[1].X * widthRatio) + (0.5f * screenWidth), (ppt.Vertices[1].Y * heightRatio) + (0.5f * screenHeight));
        Vector2 cPrime = new((ppt.Vertices[2].X * widthRatio) + (0.5f * screenWidth), (ppt.Vertices[2].Y * heightRatio) + (0.5f * screenHeight));

        return new Triangle2D(aPrime, bPrime, cPrime);
    }

    private readonly Vector3[] CalculateBoundingBox3D()
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

public struct Triangle2D
{
    public Vector2[] Vertices;

    public Triangle2D()
    {
        Vertices = [new Vector2(0f), new Vector2(0f), new Vector2(0f)];
    }

    public Triangle2D(Vector2 a, Vector2 b, Vector2 c)
    {
        Vertices = [a, b, c];
    }

    public static bool IsPointToRightOfLine(Vector2 a, Vector2 b, Vector2 point)
    {
        Vector2 aToB = Vector2.Normalize(Vector2.Subtract(b, a));
        Vector2 aToPoint = Vector2.Normalize(Vector2.Subtract(point, a));

        Vector2 abNormal = new(aToB.Y, -aToB.X);

        return Vector2.Dot(abNormal, aToPoint) >= 0f;
    }

    public readonly bool IsPointInside(Vector2 point)
    {
        bool ab = IsPointToRightOfLine(Vertices[0], Vertices[1], point);
        bool bc = IsPointToRightOfLine(Vertices[1], Vertices[2], point);
        bool ca = IsPointToRightOfLine(Vertices[2], Vertices[0], point);

        return ab && bc && ca;
    }

    public readonly void Render(Vector3 color)
    {
        for (int row = 0; row < WindowManager.WindowSizeY; row++)
        {
            for (int col = 0; col < WindowManager.WindowSizeX; col++)
            {
                if (IsPointInside(new Vector2(col, row)))
                {
                    // WindowManager.PixelGrid[row][col] = new Vector3((float)col / WindowManager.WindowSizeX, (float)row / WindowManager.WindowSizeY, 0f);
                    WindowManager.PixelGrid[row][col] = color;
                }
            }
        }
    }
}