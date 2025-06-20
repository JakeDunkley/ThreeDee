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

    public Vector3 Scale;
    public Vector3 Rotation;
    public Vector3 Translation;

    public Vector3[] TransformedVertices;
    public Vector3[] ScreenSpaceVertices;

    private static readonly double DegToRadCoef = 0.01745329251; // PI/180

    public SceneObject(string filename)
    {
        string[] lines = File.ReadLines(filename).ToArray();

        List<Vector3> parsedVertices = new();
        List<Triangle> parsedTriangles = new();

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
            }
        }

        Vertices = parsedVertices.ToArray();
        Triangles = parsedTriangles.ToArray();

        TransformedVertices = new Vector3[Vertices.Length];
        ScreenSpaceVertices = new Vector3[Vertices.Length];
    }

    private static Matrix4x4 RotationMatrixX(float degrees)
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

    private static Matrix4x4 RotationMatrixY(float degrees)
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

    private static Matrix4x4 RotationMatrixZ(float degrees)
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

    private static Vector3 RotateAboutAxis(Vector3 vertex, Matrix4x4 rot)
    {
        return Vector3.Transform(vertex, rot);
    }

    public void TransformVertices()
    {
        Matrix4x4 rotationMatrix = Matrix4x4.Multiply(RotationMatrixZ(Rotation.Z), Matrix4x4.Multiply(RotationMatrixY(Rotation.Y), RotationMatrixX(Rotation.X)));

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
            TransformedVertices[i] = Vector3.Transform(Vertices[i], transformMatrix);
        }
    }
}