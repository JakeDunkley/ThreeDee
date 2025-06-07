using System.Numerics;
using ThreeDee.Structs;

namespace ThreeDee;

public class Material
{
    public virtual Color CalculateColorAt(Vector2 screenSpaceCoordinate)
    {
        return Color.Red;
    }
}