using System.Numerics;
using ThreeDee.Structs;

namespace ThreeDee;

public class Material
{
    private readonly Color _baseColor = Color.Random;

    public virtual Color CalculateColorAt(Vector2 screenSpaceCoordinate)
    {
        return _baseColor;
    }
}