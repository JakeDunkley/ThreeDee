using System.Numerics;
using SFML.Window;
using ThreeDee.Structs;

namespace ThreeDee;

public class SceneCamera
{
    public int ResolutionX = 512;
    public int ResolutionY = 512;
    public float FOV = 90f;
    public float NearPlaneDepth = 0.01f;
    public float FarPlaneDepth = 100f;

    public float ProjectionPlaneWidth;
    public float WidthRatio;
    public float HeightRatio;

    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 BasisVectorForward;

    public float[,] DepthBuffer;
    public Color[,] ColorBuffer;

    public SceneCamera()
    {
        ProjectionPlaneWidth = 2f * NearPlaneDepth * (float)Math.Tan(0.5f * MathHelpers.DegToRadCoef * FOV);

        WidthRatio = ResolutionX / ProjectionPlaneWidth;
        HeightRatio = ResolutionY / ProjectionPlaneWidth;

        DepthBuffer = new float[ResolutionX, ResolutionY];
        ColorBuffer = new Color[ResolutionX, ResolutionY];

        BasisVectorForward = new(0, 0, 1);
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

    public void UpdateBasisVectorForward()
    {
        float cos = (float)Math.Cos(-MathHelpers.DegToRadCoef * Rotation.Y);
        float sin = (float)Math.Sin(-MathHelpers.DegToRadCoef * Rotation.Y);

        Console.WriteLine(Rotation.Y);

        BasisVectorForward = new(
            sin,
            0,
            cos
        );
    }

    public void CheckForInput()
    {
        if (Keyboard.IsKeyPressed(Keyboard.Key.W))
        {
            Position = Vector3.Add(Position, BasisVectorForward);
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.S))
        {
            Position = Vector3.Subtract(Position, BasisVectorForward);
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.A))
        {
            Position = Vector3.Add(Position, new(-BasisVectorForward.Z, 0, BasisVectorForward.X));
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.D))
        {
            Position = Vector3.Subtract(Position, new(-BasisVectorForward.Z, 0, BasisVectorForward.X));
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
        {
            Rotation = Vector3.Add(Rotation, new Vector3(0, 4f, 0));

            UpdateBasisVectorForward();
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
        {
            Rotation = Vector3.Add(Rotation, new Vector3(0, -4f, 0));

            UpdateBasisVectorForward();
        }
    }
}