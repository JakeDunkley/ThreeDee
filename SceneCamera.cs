using System.Numerics;

namespace ThreeDee;

public class SceneCamera
{
    public Vector3 Position;
    public Vector3 Rotation;

    public float FOV = 90f;
    public float NearPlaneDepth = 1f;
    public float FarPlaneDepth = 10f;
}