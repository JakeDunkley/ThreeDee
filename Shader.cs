using System.Numerics;
using SFML.Graphics;
using ThreeDee.Structs;

namespace ThreeDee;

public class Shader
{
    public virtual Structs.Color ComputeAt(float depth, Vector3 vertexWeights, int triangleIndex, SceneObject sceneObject)
    {
        return (1f - float.Pow(depth, 0.125f)) * Structs.Color.Magenta;
    }
}

public class RandomTriColorShader : Shader
{
    public override Structs.Color ComputeAt(float depth, Vector3 vertexWeights, int triangleIndex, SceneObject sceneObject)
    {
        Structs.Color[] colors = [Structs.Color.Red, Structs.Color.Yellow, Structs.Color.Green, Structs.Color.Cyan, Structs.Color.Blue];

        return (1f - depth) * colors[triangleIndex % colors.Length];
    }
}

public class TextureShader : Shader
{
    public Image Texture;

    public TextureShader(string filename)
    {
        Texture = new(filename);
    }

    public override Structs.Color ComputeAt(float depth, Vector3 vertexWeights, int triangleIndex, SceneObject sceneObject)
    {
        Vector2[] uvCoords = [
            sceneObject.UVCoordinates[sceneObject.Triangles[triangleIndex].uvA],
            sceneObject.UVCoordinates[sceneObject.Triangles[triangleIndex].uvB],
            sceneObject.UVCoordinates[sceneObject.Triangles[triangleIndex].uvC]
        ];

        float[] depths = [
            sceneObject.ScreenSpaceVertices[sceneObject.Triangles[triangleIndex].A].Z,
            sceneObject.ScreenSpaceVertices[sceneObject.Triangles[triangleIndex].B].Z,
            sceneObject.ScreenSpaceVertices[sceneObject.Triangles[triangleIndex].C].Z
        ];

        Vector2 uv = vertexWeights[0] * uvCoords[0] / depths[0];
        uv += vertexWeights[1] * uvCoords[1] / depths[1];
        uv += vertexWeights[2] * uvCoords[2] / depths[2];
        uv *= depth;

        uint textureCoordX = (uint)(uv.X * Texture.Size.X) % (Texture.Size.X - 1);
        uint textureCoordY = (uint)(uv.Y * Texture.Size.Y) % (Texture.Size.Y - 1);

        SFML.Graphics.Color color = Texture.GetPixel(textureCoordX, textureCoordY);

        return new Structs.Color(
            255 - color.R,
            255 - color.G,
            255 - color.B
        );
    }

    public Structs.Color ComputeAt_Debug(float depth, Vector3 vertexWeights, int triangleIndex, SceneObject sceneObject)
    {
        return Structs.Color.Green;
    }
}