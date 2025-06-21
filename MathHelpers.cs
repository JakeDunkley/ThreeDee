using System.Numerics;

namespace ThreeDee;

public static class MathHelpers
{
    public static readonly double DegToRadCoef = 0.01745329251; // PI/180

    public static Matrix4x4 RotationMatrixX(float degrees)
    {
        float cos = (float)Math.Cos(DegToRadCoef * degrees);
        float sin = (float)Math.Sin(DegToRadCoef * degrees);

        return new Matrix4x4(
            1, 0, 0, 0,
            0, cos, -sin, 0,
            0, sin, cos, 0,
            0, 0, 0, 1
        );
    }

    public static Matrix4x4 RotationMatrixY(float degrees)
    {
        float cos = (float)Math.Cos(DegToRadCoef * degrees);
        float sin = (float)Math.Sin(DegToRadCoef * degrees);

        return new Matrix4x4(
            cos, 0, sin, 0,
            0, 1, 0, 0,
            -sin, 0, cos, 0,
            0, 0, 0, 1
        );
    }

    public static Matrix4x4 RotationMatrixZ(float degrees)
    {
        float cos = (float)Math.Cos(DegToRadCoef * degrees);
        float sin = (float)Math.Sin(DegToRadCoef * degrees);

        return new Matrix4x4(
            cos, -sin, 0, 0,
            sin, cos, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        );
    }

    public static Vector3 TransformPoint(Vector3 point, Vector3 scale, Vector3 rotation, Vector3 translation)
    {
        Matrix4x4 rotationMatrix = Matrix4x4.Multiply(MathHelpers.RotationMatrixZ(rotation.Z), Matrix4x4.Multiply(MathHelpers.RotationMatrixY(rotation.Y), MathHelpers.RotationMatrixX(rotation.X)));

        Matrix4x4 scaleMatrix = new(
            scale.X, 0, 0, 0,
            0, scale.Y, 0, 0,
            0, 0, scale.Z, 0,
            0, 0, 0, 1
        );

        Matrix4x4 translationMatrix = new(
            1, 0, 0, translation.X,
            0, 1, 0, translation.Y,
            0, 0, 1, translation.Z,
            0, 0, 0, 1
        );

        Matrix4x4 transformMatrix = Matrix4x4.Multiply(translationMatrix, Matrix4x4.Multiply(scaleMatrix, rotationMatrix));

        return Vector3.Transform(point, transformMatrix);
    }

    public static bool IsPointToRightOfLine(Vector3 point, Vector3 a, Vector3 b)
    {
        Vector3 aToB = Vector3.Normalize(Vector3.Subtract(b, a));
        Vector3 aToPoint = Vector3.Normalize(Vector3.Subtract(point, a));

        Vector3 abNormal = new(aToB.Y, -aToB.X, 0);

        return Vector3.Dot(abNormal, aToPoint) > 0f;
    }

    public static bool IsPointInside(Vector3 point, Vector3 a, Vector3 b, Vector3 c)
    {
        bool ab = IsPointToRightOfLine(a, b, point);
        bool bc = IsPointToRightOfLine(b, c, point);
        bool ca = IsPointToRightOfLine(c, a, point);

        return (ab && bc && ca) || !(ab || bc || ca);
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

    public static float CalculateDepth(Vector3 point, Vector3 a, Vector3 b, Vector3 c)
    {
        float abp = CalculateArea(a, b, point);
        float bcp = CalculateArea(b, c, point);
        float cap = CalculateArea(c, a, point);

        float sum = abp + bcp + cap;

        return ((bcp * a.Z) + (cap * b.Z) + (abp * c.Z)) / sum;
    }
}