using System.Numerics;

namespace ThreeDee.Structs;

public readonly struct Model
{
    public readonly Triangle3D[] Triangles;
    public readonly Structs.Color[] Colors;

    public Model(Triangle3D[] triangles)
    {
        Triangles = triangles;

        Colors = new Structs.Color[triangles.Length];

        Random gen = new();

        for (int i = 0; i < Colors.Length; i++)
        {
            Colors[i] = new Structs.Color(gen);
        }
    }

    public Model(Triangle3D[] triangles, Structs.Color[] colors)
    {
        Triangles = triangles;
        Colors = colors;
    }

    public readonly void Scale(Vector3 scalars)
    {
        foreach (Triangle3D triangle in Triangles)
        {
            triangle.Scale(scalars);
        }
    }

    public readonly void Translate(Vector3 translation)
    {
        foreach (Triangle3D triangle in Triangles)
        {
            triangle.Translate(translation);
        }
    }

    public readonly void Render()
    {
        for (int i = 0; i < Triangles.Length; i++)
        {
            Triangle2D triangle2D = Triangles[i].ProjectToScreenSpace(1f, 1f, 1f, WindowManager.WindowSizeX, WindowManager.WindowSizeY);

            triangle2D.Render(Colors[i]);
        }
    }

    public static Model FromVertices(Vector3[] vertices)
    {
        Triangle3D[] tris = new Triangle3D[vertices.Length - 2];

        tris[0] = new Triangle3D(vertices[0], vertices[1], vertices[2]);

        for (int i = 3; i < vertices.Length; i++)
        {
            tris[i] = new Triangle3D(vertices[0], vertices[i - 1], vertices[i]);
        }

        return new Model(tris);
    }

    public static Model FromWavefrontFile(string filename)
    {
        string[] lines = File.ReadLines(filename).ToArray();
        List<Vector3> vertices = new();
        List<Triangle3D> triangles = new();

        foreach (string line in lines)
        {
            string[] splits = line.Split(' ');

            switch (splits[0])
            {
                case "v":
                    vertices.Add(new Vector3(
                        float.Parse(splits[1]),
                        float.Parse(splits[2]),
                        -float.Parse(splits[3])
                    ));

                    break;

                // Because of the wavefront file format, we are guaranteed that all the vertices
                // we need have already been parsed before any triangles are constructed.
                case "f":
                    triangles.Add(new Triangle3D(
                        vertices[int.Parse(splits[1].Split('/')[0]) - 1],
                        vertices[int.Parse(splits[2].Split('/')[0]) - 1],
                        vertices[int.Parse(splits[3].Split('/')[0]) - 1]
                    ));

                    break;
            }
        }

        return new Model(triangles.ToArray());
    }
}