using System.Numerics;

namespace ThreeDee;

public struct Triangle
{
    public int A, B, C;
    public int nA, nB, nC;
    public int uvA, uvB, uvC;
    public Shader Shader;
}

public class SceneObject
{
    public Vector3[] Vertices;
    public Vector3[] Normals;
    public Vector2[] UVs;
    public Triangle[] Triangles;

    public Vector3 Scale;
    public Vector3 Rotation;
    public Vector3 Translation;

    public Shader Shader;

    public List<Vector3> TransVertices;
    public List<Vector3> TransNormals;
    public List<Vector3> SSVertices;
    public List<Vector2> ClipUVs;
    public List<Triangle> ClipTriangles;

    public SceneObject(string filename, Shader shader)
    {
        string[] lines = File.ReadLines(filename).ToArray();

        Shader = shader;

        List<Vector3> parsedVertices = new();
        List<Vector3> parsedNormals = new();
        List<Triangle> parsedTriangles = new();
        List<Vector2> parsedUVCoordinates = new();

        foreach (string line in lines)
        {
            string[] splits = line.Split(' ');

            switch (splits[0])
            {
                case "v":
                    parsedVertices.Add(new Vector3(
                        float.Parse(splits[1]),
                        float.Parse(splits[2]),
                        -float.Parse(splits[3])
                    ));

                    break;

                case "vn":
                    parsedNormals.Add(new Vector3(
                        float.Parse(splits[1]),
                        float.Parse(splits[2]),
                        -float.Parse(splits[3])
                    ));

                    break;

                case "vt":
                    parsedUVCoordinates.Add(new Vector2(
                        float.Parse(splits[1]),
                        1f - float.Parse(splits[2])
                    ));

                    break;

                case "f":
                    string[] v1Splits = splits[1].Split('/');
                    string[] v2Splits = splits[2].Split('/');
                    string[] v3Splits = splits[3].Split('/');

                    parsedTriangles.Add(
                        new Triangle
                        {
                            A = int.Parse(v1Splits[0]) - 1,
                            B = int.Parse(v2Splits[0]) - 1,
                            C = int.Parse(v3Splits[0]) - 1,
                            nA = int.Parse(v1Splits[2]) - 1,
                            nB = int.Parse(v2Splits[2]) - 1,
                            nC = int.Parse(v3Splits[2]) - 1,
                            uvA = int.Parse(v1Splits[1]) - 1,
                            uvB = int.Parse(v2Splits[1]) - 1,
                            uvC = int.Parse(v3Splits[1]) - 1,
                            Shader = Shader
                        }
                    );

                    break;
            }
        }

        Vertices = parsedVertices.ToArray();
        Normals = parsedNormals.ToArray();
        UVs = parsedUVCoordinates.ToArray();
        Triangles = parsedTriangles.ToArray();

        TransVertices = new List<Vector3>(new Vector3[Vertices.Length]);
        TransNormals = new();
        SSVertices = new();
        ClipUVs = new();
        ClipTriangles = new();

        Scale = new Vector3(1);
    }

    private void CopyUVs()
    {
        foreach (Vector2 uv in UVs)
        {
            ClipUVs.Add(uv);
        }
    }

    public void ClearProcessedGeometryBuffers()
    {
        TransVertices = new List<Vector3>(new Vector3[Vertices.Length]);
        TransNormals = new List<Vector3>(new Vector3[Normals.Length]);
        SSVertices.Clear();
        ClipUVs.Clear();
        CopyUVs();
        ClipTriangles.Clear();
    }

    public void ScaleBy(Vector3 scaleAddition)
    {
        Scale += scaleAddition;
    }

    public void RotateBy(Vector3 rotationAddition)
    {
        Rotation += rotationAddition;
    }

    public void TranslateBy(Vector3 translationAddition)
    {
        Translation += translationAddition;
    }

    public void TransformParallel(SceneCamera camera)
    {
        Matrix4x4 rotationMatrix = Matrix4x4.Multiply(MathHelpers.RotationMatrixZ(Rotation.Z), Matrix4x4.Multiply(MathHelpers.RotationMatrixY(Rotation.Y), MathHelpers.RotationMatrixX(Rotation.X)));

        Matrix4x4 scaleMatrix = new(
            Scale.X, 0, 0, 0,
            0, Scale.Y, 0, 0,
            0, 0, Scale.Z, 0,
            0, 0, 0, 1
        );

        Matrix4x4 translationMatrix = new(
            1, 0, 0, Translation.X - camera.Position.X,
            0, 1, 0, Translation.Y - camera.Position.Y,
            0, 0, 1, Translation.Z - camera.Position.Z,
            0, 0, 0, 1
        );

        Matrix4x4 transformMatrix = Matrix4x4.Multiply(translationMatrix, Matrix4x4.Multiply(scaleMatrix, rotationMatrix));

        Matrix4x4 cameraRotationMatrix = Matrix4x4.Multiply(MathHelpers.RotationMatrixZ(camera.Rotation.Z), Matrix4x4.Multiply(MathHelpers.RotationMatrixY(camera.Rotation.Y), MathHelpers.RotationMatrixX(camera.Rotation.X)));

        transformMatrix = Matrix4x4.Multiply(cameraRotationMatrix, transformMatrix);

        Matrix4x4 normalRotationMatrix = Matrix4x4.Multiply(cameraRotationMatrix, rotationMatrix);

        Parallel.For(0, Vertices.Length, i =>
        {
            Matrix4x4 vertexMatrix = new(
                Vertices[i].X, 0, 0, 0,
                Vertices[i].Y, 0, 0, 0,
                Vertices[i].Z, 0, 0, 0,
                1, 0, 0, 0
            );

            Matrix4x4 transformedVertexMatrix = Matrix4x4.Multiply(transformMatrix, vertexMatrix);

            TransVertices[i] = new Vector3(transformedVertexMatrix.M11, transformedVertexMatrix.M21, transformedVertexMatrix.M31);
        });

        Parallel.For(0, Normals.Length, i =>
        {
            Matrix4x4 normalMatrix = new(
                Normals[i].X, 0, 0, 0,
                Normals[i].Y, 0, 0, 0,
                Normals[i].Z, 0, 0, 0,
                1, 0, 0, 0
            );

            Matrix4x4 transformedNormalMatrix = Matrix4x4.Multiply(normalRotationMatrix, normalMatrix);

            TransNormals[i] = new Vector3(transformedNormalMatrix.M11, transformedNormalMatrix.M21, transformedNormalMatrix.M31);
        });
    }

    public void ProjectToScreenSpace(SceneCamera camera)
    {
        float halfResX = 0.5f * camera.ResolutionX;
        float halfResY = 0.5f * camera.ResolutionY;

        for (int i = 0; i < TransVertices.Count; i++)
        {
            float zRatio = camera.NearPlaneDepth / TransVertices[i].Z;

            SSVertices.Add(new Vector3(
                (TransVertices[i].X * camera.WidthRatio * zRatio) + halfResX,
                (TransVertices[i].Y * camera.HeightRatio * zRatio) + halfResY,
                (TransVertices[i].Z - camera.NearPlaneDepth) / (camera.FarPlaneDepth - camera.NearPlaneDepth)
            ));
        }
    }

    public int[] CalculatePixelScreenSpaceBounds(Triangle triangle, SceneCamera camera)
    {
        Vector3 min = Vector3.Min(SSVertices[triangle.A], Vector3.Min(SSVertices[triangle.B], SSVertices[triangle.C]));
        Vector3 max = Vector3.Max(SSVertices[triangle.A], Vector3.Max(SSVertices[triangle.B], SSVertices[triangle.C]));

        return [
            Math.Max(0, (int)Math.Floor(min.X)),
            Math.Max(0, (int)Math.Floor(min.Y)),
            Math.Min(camera.ResolutionX - 1, (int)Math.Ceiling(max.X)),
            Math.Min(camera.ResolutionY - 1, (int)Math.Ceiling(max.Y))
        ];
    }
}