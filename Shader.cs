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

public class TextureShader : Shader
{
    public Image Texture;

    public TextureShader(string filename)
    {
        Texture = new(filename);
    }

    public override Structs.Color ComputeAt(float depth, Vector3 vertexWeights, int triangleIndex, SceneObject sceneObject)
    {
        Vector2[] textureCoords = [
            sceneObject.UVCoordinates[sceneObject.Triangles[triangleIndex].A],
            sceneObject.UVCoordinates[sceneObject.Triangles[triangleIndex].B],
            sceneObject.UVCoordinates[sceneObject.Triangles[triangleIndex].C]
        ];

        Vector2 uv = depth *
            ((vertexWeights[0] * (textureCoords[0] / sceneObject.ScreenSpaceVertices[sceneObject.Triangles[triangleIndex].A].Z))
            + (vertexWeights[1] * (textureCoords[1] / sceneObject.ScreenSpaceVertices[sceneObject.Triangles[triangleIndex].B].Z))
            + (vertexWeights[2] * (textureCoords[2] / sceneObject.ScreenSpaceVertices[sceneObject.Triangles[triangleIndex].C].Z)));

        uint uvx = uint.Min(Texture.Size.X - 1, uint.Max(0, (uint)(uv.X * Texture.Size.X)));
        uint uvy = uint.Min(Texture.Size.Y - 1, uint.Max(0, (uint)(uv.Y * Texture.Size.Y)));

        SFML.Graphics.Color color = Texture.GetPixel(uvx, uvy);

        return new Structs.Color(
            255 - color.R,
            255 - color.G,
            255 - color.B
        );
    }

    public Structs.Color ComputeAt_Debug(float depth, Vector3 vertexWeights, int triangleIndex, SceneObject sceneObject)
    {
        Vector2[] textureCoords = [
            sceneObject.UVCoordinates[sceneObject.Triangles[triangleIndex].A],
            sceneObject.UVCoordinates[sceneObject.Triangles[triangleIndex].B],
            sceneObject.UVCoordinates[sceneObject.Triangles[triangleIndex].C]
        ];

        Vector2 uv = depth *
        ((vertexWeights[0] * (textureCoords[0] / sceneObject.ScreenSpaceVertices[sceneObject.Triangles[triangleIndex].A].Z))
        + (vertexWeights[1] * (textureCoords[1] / sceneObject.ScreenSpaceVertices[sceneObject.Triangles[triangleIndex].B].Z))
        + (vertexWeights[2] * (textureCoords[2] / sceneObject.ScreenSpaceVertices[sceneObject.Triangles[triangleIndex].C].Z)));

        // return new Structs.Color(uv.X, uv.Y, 0);

        return new Structs.Color((float)triangleIndex / (sceneObject.Triangles.Length - 1));
    }
}