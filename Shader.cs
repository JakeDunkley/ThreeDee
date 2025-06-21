using System.Numerics;
using SFML.Graphics;
using ThreeDee.Structs;

namespace ThreeDee;

public class Shader
{
    public virtual Structs.Color ComputeAt(Vector3 ssPoint)
    {
        return Structs.Color.Magenta;
    }
}

public class TextureShader : Shader
{
    public Texture Texture;

    public TextureShader(string filename)
    {
        Texture = new(filename);
    }

    public override Structs.Color ComputeAt(Vector3 ssPoint)
    {
        return base.ComputeAt(ssPoint);
    }
}