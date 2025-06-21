using System.Numerics;
using ThreeDee.Structs;

namespace ThreeDee;

public class SceneCamera
{
    public int ResolutionX = 256;
    public int ResolutionY = 256;
    public float FOV = 90f;
    public float NearPlaneDepth = 1f;
    public float FarPlaneDepth = 10f;

    public float ProjectionPlaneWidth;
    public float WidthRatio;
    public float HeightRatio;

    public Vector3 Position;
    public Vector3 Rotation;

    public float[,] DepthBuffer;
    public Color[,] ColorBuffer;

    public SceneCamera()
    {
        ProjectionPlaneWidth = 2f * NearPlaneDepth * (float)Math.Tan(0.5f * MathHelpers.DegToRadCoef * FOV);

        WidthRatio = ResolutionX / ProjectionPlaneWidth;
        HeightRatio = ResolutionY / ProjectionPlaneWidth;

        DepthBuffer = new float[ResolutionX, ResolutionY];
        ColorBuffer = new Color[ResolutionX, ResolutionY];
    }

    public void ClearBuffers()
    {
        for (int row = 0; row < ResolutionY; row++)
        {
            for (int col = 0; col < ResolutionX; col++)
            {
                DepthBuffer[row, col] = 0f;
                ColorBuffer[row, col] = Color.Black;
            }
        }
    }
}