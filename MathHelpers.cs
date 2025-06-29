using System.Numerics;
using SFML.Graphics;

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

    public static bool IsInside(Vector3 weights)
    {
        return weights.X >= 0f && weights.Y >= 0f && weights.Z >= 0f && Vector3.Dot(weights, weights) > 0f;
    }

    public static float CalculateArea(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 ca = Vector3.Subtract(a, c);
        Vector3 rotCA = new(-ca.Y, ca.X, 0);

        Vector3 cb = Vector3.Subtract(b, c);

        float dot = Vector3.Dot(rotCA, cb);
        float projCoef = dot / Vector3.Dot(rotCA, rotCA);
        Vector2 proj = new(projCoef * rotCA.X, projCoef * rotCA.Y);

        return 0.5f * float.Sign(dot) * ca.Length() * proj.Length();
    }

    public static Vector3 CalculateVertexWeights(Vector3 point, Vector3 a, Vector3 b, Vector3 c)
    {
        float ab = CalculateArea(a, b, point);
        float bc = CalculateArea(b, c, point);
        float ca = CalculateArea(c, a, point);

        float sum = ab + bc + ca;

        if (sum <= 0)
        {
            return Vector3.Zero;
        }

        return Vector3.Divide(new(bc, ca, ab), ab + bc + ca);
    }

    public static float CalculateDepth(Vector3 weights, Vector3 a, Vector3 b, Vector3 c)
    {
        return 1f / Vector3.Dot(weights, new(1f / a.Z, 1f / b.Z, 1f / c.Z));
    }

    public static Vector3 CalculateNormalSmooth(Vector3 weights, Triangle triangle, SceneObject sceneObject)
    {
        Vector3[] normals = [
            sceneObject.TransformedNormals[triangle.nA],
            sceneObject.TransformedNormals[triangle.nB],
            sceneObject.TransformedNormals[triangle.nC]
        ];

        return (weights[0] * normals[0]) + (weights[1] * normals[1]) + (weights[2] * normals[2]);
    }

    public static bool IsTriangleLeftOfFrustum(Triangle triangle, SceneObject sceneObject, SceneCamera camera)
    {
        return sceneObject.ScreenSpaceVertices[triangle.A].X < 0
        && sceneObject.ScreenSpaceVertices[triangle.B].X < 0
        && sceneObject.ScreenSpaceVertices[triangle.C].X < 0;
    }

    public static bool IsTriangleRightOfFrustum(Triangle triangle, SceneObject sceneObject, SceneCamera camera)
    {
        return sceneObject.ScreenSpaceVertices[triangle.A].X > camera.ResolutionX
        && sceneObject.ScreenSpaceVertices[triangle.B].X > camera.ResolutionX
        && sceneObject.ScreenSpaceVertices[triangle.C].X > camera.ResolutionX;
    }

    public static bool IsTriangleAboveFrustum(Triangle triangle, SceneObject sceneObject, SceneCamera camera)
    {
        return sceneObject.ScreenSpaceVertices[triangle.A].Y > camera.ResolutionY
        && sceneObject.ScreenSpaceVertices[triangle.B].Y > camera.ResolutionY
        && sceneObject.ScreenSpaceVertices[triangle.C].Y > camera.ResolutionY;
    }

    public static bool IsTriangleBelowFrustum(Triangle triangle, SceneObject sceneObject, SceneCamera camera)
    {
        return sceneObject.ScreenSpaceVertices[triangle.A].X < 0
        && sceneObject.ScreenSpaceVertices[triangle.B].X < 0
        && sceneObject.ScreenSpaceVertices[triangle.C].X < 0;
    }

    public static bool IsTriangleOutsideFrustum(Triangle triangle, SceneObject sceneObject, SceneCamera camera)
    {
        return IsTriangleLeftOfFrustum(triangle, sceneObject, camera)
        || IsTriangleRightOfFrustum(triangle, sceneObject, camera)
        || IsTriangleAboveFrustum(triangle, sceneObject, camera)
        || IsTriangleBelowFrustum(triangle, sceneObject, camera);
    }

    public static bool IsVertexBehindNearPlane(Vector3 vertex, SceneCamera camera)
    {
        return vertex.Z <= camera.NearPlaneDepth;
    }

    public static bool IsTriangleBehindNearPlane(Triangle triangle, SceneObject sceneObject, SceneCamera camera)
    {
        return IsVertexBehindNearPlane(sceneObject.TransformedVertices[triangle.A], camera)
        || IsVertexBehindNearPlane(sceneObject.TransformedVertices[triangle.B], camera)
        || IsVertexBehindNearPlane(sceneObject.TransformedVertices[triangle.C], camera);
    }

    private static void DivideTriangleMissingOneVertex(Triangle triangle, SceneObject sceneObject, SceneCamera camera)
    {

    }

    private static void DivideTriangleMissingTwoVertices(Triangle triangle, SceneObject sceneObject, SceneCamera camera)
    {
        float distAToNearPlane = camera.NearPlaneDepth - sceneObject.TransformedVertices[triangle.A].Z;
        float distCToNearPlane = camera.NearPlaneDepth - sceneObject.TransformedVertices[triangle.C].Z;

        float distAToB = sceneObject.TransformedVertices[triangle.B].Z - sceneObject.TransformedVertices[triangle.A].Z;
        float distCToB = sceneObject.TransformedVertices[triangle.B].Z - sceneObject.TransformedVertices[triangle.C].Z;

        float tAB = distAToNearPlane / distAToB;
        float tCB = distCToNearPlane / distCToB;

        // Vector3 newA = (tAB * sceneObject.TransformedVertices[triangle.A]) + ((1f - tAB) * sceneObject.TransformedVertices[triangle.B]);
        // Vector3 newC = (tCB * sceneObject.TransformedVertices[triangle.C]) + ((1f - tCB) * sceneObject.TransformedVertices[triangle.B]);
        Vector3 newA = ((1f - tAB) * sceneObject.TransformedVertices[triangle.A]) + (tAB * sceneObject.TransformedVertices[triangle.B]);
        Vector3 newC = ((1f - tCB) * sceneObject.TransformedVertices[triangle.C]) + (tCB * sceneObject.TransformedVertices[triangle.B]);

        sceneObject.TransformedVertices.Add(newA);
        sceneObject.TransformedVertices.Add(newC);

        Triangle clippedTriangle = new()
        {
            A = sceneObject.TransformedVertices.Count - 2,
            B = triangle.B,
            C = sceneObject.TransformedVertices.Count - 1
        };

        sceneObject.ClippedTriangles.Add(clippedTriangle);
    }

    public static void ClipTriangleMissingTwoVertices(bool isAB, bool isBC, bool isCA, Triangle triangle, SceneObject sceneObject, SceneCamera camera)
    {
        if (isAB)
        {
            DivideTriangleMissingTwoVertices(new Triangle{A = triangle.B, B = triangle.C, C = triangle.A}, sceneObject, camera);
            return;
        }

        if (isBC)
        {
            DivideTriangleMissingTwoVertices(new Triangle{A = triangle.C, B = triangle.A, C = triangle.B}, sceneObject, camera);
            return;
        }

        if (isCA)
        {
            DivideTriangleMissingTwoVertices(triangle, sceneObject, camera);
            return;
        }
    }
}