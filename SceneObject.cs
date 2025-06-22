using System.Numerics;

namespace ThreeDee;

public struct Triangle
{
    public int A, B, C;
}

public class SceneObject
{
    public Vector3[] Vertices;
    public Triangle[] Triangles;
    public Vector2[] UVCoordinates;

    public Vector3 Scale;
    public Vector3 Rotation;
    public Vector3 Translation;

    public Shader Shader;

    public Vector3[] TransformedVertices;
    public Vector3[] ScreenSpaceVertices;

    public SceneObject(string filename)
    {
        string[] lines = File.ReadLines(filename).ToArray();

        List<Vector3> parsedVertices = new();
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

                case "f":
                    parsedTriangles.Add(
                        new Triangle
                        {
                            A = int.Parse(splits[1].Split('/')[0]) - 1,
                            B = int.Parse(splits[2].Split('/')[0]) - 1,
                            C = int.Parse(splits[3].Split('/')[0]) - 1
                        }
                    );

                    break;

                case "vt":
                    parsedUVCoordinates.Add(new Vector2(
                        float.Parse(splits[1]),
                        float.Parse(splits[2])
                    ));

                    break;
            }
        }

        Vertices = parsedVertices.ToArray();
        Triangles = parsedTriangles.ToArray();
        UVCoordinates = parsedUVCoordinates.ToArray();

        TransformedVertices = new Vector3[Vertices.Length];
        ScreenSpaceVertices = new Vector3[Vertices.Length];

        Scale = new Vector3(1);

        Shader = new();
    }

    public void ScaleBy(Vector3 scaleAddition)
    {
        Scale = Vector3.Add(Scale, scaleAddition);
    }

    public void RotateBy(Vector3 rotationAddition)
    {
        Rotation = Vector3.Add(Rotation, rotationAddition);
    }

    public void TranslateBy(Vector3 translationAddition)
    {
        Translation = Vector3.Add(Translation, translationAddition);
    }

    public void Transform()
    {
        Matrix4x4 rotationMatrix = Matrix4x4.Multiply(MathHelpers.RotationMatrixZ(Rotation.Z), Matrix4x4.Multiply(MathHelpers.RotationMatrixY(Rotation.Y), MathHelpers.RotationMatrixX(Rotation.X)));

        Matrix4x4 scaleMatrix = new(
            Scale.X, 0, 0, 0,
            0, Scale.Y, 0, 0,
            0, 0, Scale.Z, 0,
            0, 0, 0, 1
        );

        Matrix4x4 translationMatrix = new(
            1, 0, 0, Translation.X,
            0, 1, 0, Translation.Y,
            0, 0, 1, Translation.Z,
            0, 0, 0, 1
        );

        Matrix4x4 transformMatrix = Matrix4x4.Multiply(translationMatrix, Matrix4x4.Multiply(scaleMatrix, rotationMatrix));

        for (int i = 0; i < Vertices.Length; i++)
        {
            Matrix4x4 vertexMatrix = new(
                Vertices[i].X, 0, 0, 0,
                Vertices[i].Y, 0, 0, 0,
                Vertices[i].Z, 0, 0, 0,
                1, 0, 0, 0
            );

            Matrix4x4 transformedVertexMatrix = Matrix4x4.Multiply(transformMatrix, vertexMatrix);

            TransformedVertices[i] = new Vector3(transformedVertexMatrix.M11, transformedVertexMatrix.M21, transformedVertexMatrix.M31);
        }
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

        Parallel.For(0, Vertices.Length, i =>
        {
            Matrix4x4 vertexMatrix = new(
                Vertices[i].X, 0, 0, 0,
                Vertices[i].Y, 0, 0, 0,
                Vertices[i].Z, 0, 0, 0,
                1, 0, 0, 0
            );

            Matrix4x4 transformedVertexMatrix = Matrix4x4.Multiply(transformMatrix, vertexMatrix);

            TransformedVertices[i] = new Vector3(transformedVertexMatrix.M11, transformedVertexMatrix.M21, transformedVertexMatrix.M31);
        });
    }

    public void ProjectToScreenSpace(SceneCamera camera)
    {
        float halfResX = 0.5f * camera.ResolutionX;
        float halfResY = 0.5f * camera.ResolutionY;

        for (int i = 0; i < TransformedVertices.Length; i++)
        {
            float zRatio = camera.NearPlaneDepth / TransformedVertices[i].Z;

            ScreenSpaceVertices[i] = new Vector3(
                (TransformedVertices[i].X * camera.WidthRatio * zRatio) + halfResX,
                (TransformedVertices[i].Y * camera.HeightRatio * zRatio) + halfResY,
                (TransformedVertices[i].Z - camera.NearPlaneDepth) / (camera.FarPlaneDepth - camera.NearPlaneDepth)
            );
        }
    }

    public void ProjectToScreenSpaceParallel(SceneCamera camera)
    {
        float halfResX = 0.5f * camera.ResolutionX;
        float halfResY = 0.5f * camera.ResolutionY;

        Parallel.For(0, TransformedVertices.Length, i =>
        {
            float zRatio = camera.NearPlaneDepth / TransformedVertices[i].Z;

            ScreenSpaceVertices[i] = new Vector3(
                (TransformedVertices[i].X * camera.WidthRatio * zRatio) + halfResX,
                (TransformedVertices[i].Y * camera.HeightRatio * zRatio) + halfResY,
                (TransformedVertices[i].Z - camera.NearPlaneDepth) / (camera.FarPlaneDepth - camera.NearPlaneDepth)
            );
        });
    }

    public int[] CalculatePixelScreenSpaceBounds(Triangle triangle, SceneCamera camera)
    {
        Vector3 min = Vector3.Min(ScreenSpaceVertices[triangle.A], Vector3.Min(ScreenSpaceVertices[triangle.B], ScreenSpaceVertices[triangle.C]));
        Vector3 max = Vector3.Max(ScreenSpaceVertices[triangle.A], Vector3.Max(ScreenSpaceVertices[triangle.B], ScreenSpaceVertices[triangle.C]));

        return [
            Math.Max(0, (int)Math.Floor(min.X)),
            Math.Max(0, (int)Math.Floor(min.Y)),
            Math.Min(camera.ResolutionX - 1, (int)Math.Ceiling(max.X)),
            Math.Min(camera.ResolutionY - 1, (int)Math.Ceiling(max.Y))
        ];
    }
}