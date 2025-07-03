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
            sceneObject.TransNormals[triangle.nA],
            sceneObject.TransNormals[triangle.nB],
            sceneObject.TransNormals[triangle.nC]
        ];

        return (weights[0] * normals[0]) + (weights[1] * normals[1]) + (weights[2] * normals[2]);
    }

    public static bool IsTriangleLeftOfFrustum(Triangle triangle, SceneObject sceneObject, SceneCamera camera)
    {
        return sceneObject.SSVertices[triangle.A].X < 0
        && sceneObject.SSVertices[triangle.B].X < 0
        && sceneObject.SSVertices[triangle.C].X < 0;
    }

    public static bool IsTriangleRightOfFrustum(Triangle triangle, SceneObject sceneObject, SceneCamera camera)
    {
        return sceneObject.SSVertices[triangle.A].X > camera.ResolutionX
        && sceneObject.SSVertices[triangle.B].X > camera.ResolutionX
        && sceneObject.SSVertices[triangle.C].X > camera.ResolutionX;
    }

    public static bool IsTriangleAboveFrustum(Triangle triangle, SceneObject sceneObject, SceneCamera camera)
    {
        return sceneObject.SSVertices[triangle.A].Y > camera.ResolutionY
        && sceneObject.SSVertices[triangle.B].Y > camera.ResolutionY
        && sceneObject.SSVertices[triangle.C].Y > camera.ResolutionY;
    }

    public static bool IsTriangleBelowFrustum(Triangle triangle, SceneObject sceneObject, SceneCamera camera)
    {
        return sceneObject.SSVertices[triangle.A].X < 0
        && sceneObject.SSVertices[triangle.B].X < 0
        && sceneObject.SSVertices[triangle.C].X < 0;
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
        return IsVertexBehindNearPlane(sceneObject.TransVertices[triangle.A], camera)
        || IsVertexBehindNearPlane(sceneObject.TransVertices[triangle.B], camera)
        || IsVertexBehindNearPlane(sceneObject.TransVertices[triangle.C], camera);
    }

    private static void DivideTriangleMissingOneVertex(Triangle triangle, SceneObject sceneObject, SceneCamera camera)
    {
        float distBToNearPlane = camera.ClippingPlaneDepth - sceneObject.TransVertices[triangle.B].Z;

        float distBToA = sceneObject.TransVertices[triangle.A].Z - sceneObject.TransVertices[triangle.B].Z;
        float distBToC = sceneObject.TransVertices[triangle.C].Z - sceneObject.TransVertices[triangle.B].Z;

        float tBA = distBToNearPlane / distBToA;
        float tBC = distBToNearPlane / distBToC;

        Vector3 newBL = ((1f - tBA) * sceneObject.TransVertices[triangle.B]) + (tBA * sceneObject.TransVertices[triangle.A]);
        Vector3 newBR = ((1f - tBC) * sceneObject.TransVertices[triangle.B]) + (tBC * sceneObject.TransVertices[triangle.C]);

        sceneObject.TransVertices.Add(newBL);
        sceneObject.TransVertices.Add(newBR);

        Vector3 newBLn = ((1f - tBA) * sceneObject.TransNormals[triangle.nB]) + (tBA * sceneObject.TransNormals[triangle.nA]);
        Vector3 newBRn = ((1f - tBC) * sceneObject.TransNormals[triangle.nB]) + (tBC * sceneObject.TransNormals[triangle.nC]);

        sceneObject.TransNormals.Add(newBLn);
        sceneObject.TransNormals.Add(newBRn);

        Vector2 newBLuv = ((1f - tBA) * sceneObject.UVs[triangle.uvB]) + (tBA * sceneObject.UVs[triangle.uvA]);
        Vector2 newBRuv = ((1f - tBC) * sceneObject.UVs[triangle.uvB]) + (tBC * sceneObject.UVs[triangle.uvC]);

        sceneObject.ClipUVs.Add(newBLuv);
        sceneObject.ClipUVs.Add(newBRuv);

        Triangle clippedTriangleLeft = new()
        {
            A = triangle.A,
            B = sceneObject.TransVertices.Count - 1,
            C = triangle.C,

            nA = triangle.nA,
            nB = sceneObject.TransNormals.Count - 1,
            nC = triangle.nC,

            uvA = triangle.uvA,
            uvB = sceneObject.ClipUVs.Count - 1,
            uvC = triangle.uvC,

            Shader = new ColorShader(Structs.Color.Cyan)
        };

        Triangle clippedTriangleRight = new()
        {
            A = triangle.A,
            B = sceneObject.TransVertices.Count - 2,
            C = sceneObject.TransVertices.Count - 1,

            nA = triangle.nA,
            nB = sceneObject.TransNormals.Count - 2,
            nC = sceneObject.TransNormals.Count - 1,

            uvA = triangle.uvA,
            uvB = sceneObject.ClipUVs.Count - 2,
            uvC = sceneObject.ClipUVs.Count - 1,

            Shader = new ColorShader(Structs.Color.Green)
        };

        sceneObject.ClipTriangles.Add(clippedTriangleLeft);
        sceneObject.ClipTriangles.Add(clippedTriangleRight);
    }

    private static void DivideTriangleMissingTwoVertices(Triangle triangle, SceneObject sceneObject, SceneCamera camera)
    {
        float distAToNearPlane = camera.ClippingPlaneDepth - sceneObject.TransVertices[triangle.A].Z;
        float distCToNearPlane = camera.ClippingPlaneDepth - sceneObject.TransVertices[triangle.C].Z;

        float distAToB = sceneObject.TransVertices[triangle.B].Z - sceneObject.TransVertices[triangle.A].Z;
        float distCToB = sceneObject.TransVertices[triangle.B].Z - sceneObject.TransVertices[triangle.C].Z;

        float tAB = distAToNearPlane / distAToB;
        float tCB = distCToNearPlane / distCToB;

        Vector3 newA = ((1f - tAB) * sceneObject.TransVertices[triangle.A]) + (tAB * sceneObject.TransVertices[triangle.B]);
        Vector3 newC = ((1f - tCB) * sceneObject.TransVertices[triangle.C]) + (tCB * sceneObject.TransVertices[triangle.B]);

        sceneObject.TransVertices.Add(-newA);
        sceneObject.TransVertices.Add(-newC);

        Vector3 newAn = ((1f - tAB) * sceneObject.TransNormals[triangle.nA]) + (tAB * sceneObject.TransNormals[triangle.nB]);
        Vector3 newCn = ((1f - tCB) * sceneObject.TransNormals[triangle.nC]) + (tCB * sceneObject.TransNormals[triangle.nB]);

        sceneObject.TransNormals.Add(newAn);
        sceneObject.TransNormals.Add(newCn);

        Vector2 newAuv = ((1f - tAB) * sceneObject.UVs[triangle.uvA]) + (tAB * sceneObject.UVs[triangle.uvB]);
        Vector2 newCuv = ((1f - tCB) * sceneObject.UVs[triangle.uvC]) + (tCB * sceneObject.UVs[triangle.uvB]);

        sceneObject.ClipUVs.Add(newAuv);
        sceneObject.ClipUVs.Add(newCuv);

        Triangle clippedTriangle = new()
        {
            A = sceneObject.TransVertices.Count - 2,
            B = triangle.B,
            C = sceneObject.TransVertices.Count - 1,
            nA = sceneObject.TransNormals.Count - 2,
            nB = triangle.nB,
            nC = sceneObject.TransNormals.Count - 1,
            uvA = sceneObject.ClipUVs.Count - 2,
            uvB = triangle.uvB,
            uvC = sceneObject.ClipUVs.Count - 1,
            Shader = new ColorShader(Structs.Color.Yellow)
        };

        sceneObject.ClipTriangles.Add(clippedTriangle);
    }

    public static void ClipTriangleMissingOneVertex(bool isA, bool isB, bool isC, Triangle triangle, SceneObject sceneObject, SceneCamera camera)
    {
        if (isA)
        {
            DivideTriangleMissingOneVertex(new Triangle { A = triangle.C, B = triangle.A, C = triangle.B }, sceneObject, camera);
            return;
        }

        if (isB)
        {
            DivideTriangleMissingOneVertex(triangle, sceneObject, camera);
            return;
        }

        if (isC)
        {
            DivideTriangleMissingOneVertex(new Triangle { A = triangle.B, B = triangle.C, C = triangle.A }, sceneObject, camera);
            return;
        }
    }

    public static void ClipTriangleMissingTwoVertices(bool isAB, bool isBC, bool isCA, Triangle triangle, SceneObject sceneObject, SceneCamera camera)
    {
        if (isAB)
        {
            DivideTriangleMissingTwoVertices(new Triangle { A = triangle.B, B = triangle.C, C = triangle.A }, sceneObject, camera);
            return;
        }

        if (isBC)
        {
            DivideTriangleMissingTwoVertices(new Triangle { A = triangle.C, B = triangle.A, C = triangle.B }, sceneObject, camera);
            return;
        }

        if (isCA)
        {
            DivideTriangleMissingTwoVertices(triangle, sceneObject, camera);
            return;
        }
    }
}