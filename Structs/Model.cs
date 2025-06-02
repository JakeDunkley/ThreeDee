using System.Numerics;
using SFML.System;

namespace ThreeDee.Structs;

public readonly struct Model
{
    public readonly Triangle3D[] Triangles;
    public readonly Vector3[] Colors;

    public Model(Triangle3D[] triangles)
    {
        Triangles = triangles;

        Colors = new Vector3[triangles.Length];
        Vector3 magenta = new(1, 0, 1);

        for (int i = 0; i < Colors.Length; i++)
        {
            Colors[i] = magenta;
        }
    }

    public Model(Triangle3D[] triangles, Vector3[] colors)
    {
        Triangles = triangles;
        Colors = colors;
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
}