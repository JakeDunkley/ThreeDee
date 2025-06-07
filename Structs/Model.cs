using System.Numerics;

namespace ThreeDee.Structs;

public readonly struct Model
{
    public readonly List<int> VertexBufferIndices = new();
    public readonly List<int> TriangleBufferIndices = new();
    public readonly List<int> MaterialBufferIndices = new();

    public Model(string filename)
    {
        string[] lines = File.ReadLines(filename).ToArray();

        int worldVertexBufferStartIndex = RenderManager.WorldVertexBuffer.Count;

        foreach (string line in lines)
        {
            string[] splits = line.Split(' ');

            switch (splits[0])
            {
                case "v":
                    RenderManager.WorldVertexBuffer.Add(new Vector3(
                        float.Parse(splits[1]),
                        float.Parse(splits[2]),
                        -float.Parse(splits[3])
                    ));

                    VertexBufferIndices.Add(RenderManager.WorldVertexBuffer.Count - 1);

                    break;

                case "f":
                    RenderManager.WorldTriangleBuffer.Add(new Triangle3D(
                        worldVertexBufferStartIndex + int.Parse(splits[1].Split('/')[0]) - 1,
                        worldVertexBufferStartIndex + int.Parse(splits[2].Split('/')[0]) - 1,
                        worldVertexBufferStartIndex + int.Parse(splits[3].Split('/')[0]) - 1
                    ));

                    TriangleBufferIndices.Add(RenderManager.WorldTriangleBuffer.Count - 1);

                    break;
            }
        }
    }

    public Model(Vector3[] vertices)
    {
        int worldVertexBufferStartIndex = RenderManager.WorldVertexBuffer.Count;

        RenderManager.WorldVertexBuffer.Add(vertices[0]);
        RenderManager.WorldVertexBuffer.Add(vertices[1]);
        RenderManager.WorldVertexBuffer.Add(vertices[2]);

        RenderManager.WorldTriangleBuffer.Add(new Triangle3D(
            worldVertexBufferStartIndex,
            worldVertexBufferStartIndex + 1,
            worldVertexBufferStartIndex + 2
        ));

        for (int i = 3; i < vertices.Length; i++)
        {
            RenderManager.WorldVertexBuffer.Add(vertices[i]);

            RenderManager.WorldTriangleBuffer.Add(new Triangle3D(
                worldVertexBufferStartIndex,
                worldVertexBufferStartIndex + i - 1,
                worldVertexBufferStartIndex + i
            ));
        }
    }

    public void Translate(Vector3 translation)
    {
        foreach (int vertexBufferIndex in VertexBufferIndices)
        {
            RenderManager.WorldVertexBuffer[vertexBufferIndex] = Vector3.Add(translation, RenderManager.WorldVertexBuffer[vertexBufferIndex]);
        }
    }

    public void Scale(Vector3 scalars)
    {
        foreach (int vertexBufferIndex in VertexBufferIndices)
        {
            RenderManager.WorldVertexBuffer[vertexBufferIndex] = Vector3.Multiply(scalars, RenderManager.WorldVertexBuffer[vertexBufferIndex]);
        }
    }
}