using System.Numerics;
using SFML.Window;
using ThreeDee.Structs;

namespace ThreeDee;

public class SceneCamera
{
    public int ResolutionX = 512;
    public int ResolutionY = 512;
    public float FOV = 90f;
    public float ClippingOffset = 0.1f;
    public float NearPlaneDepth = 0.01f;
    public float FarPlaneDepth = 20f;
    public bool IsDebugRender = false;

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

        DepthBuffer = new float[ResolutionY, ResolutionX];
        ColorBuffer = new Color[ResolutionY, ResolutionX];

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

    private void UpdateBasisVectorForward()
    {
        float cos = (float)Math.Cos(-MathHelpers.DegToRadCoef * Rotation.Y);
        float sin = (float)Math.Sin(-MathHelpers.DegToRadCoef * Rotation.Y);

        BasisVectorForward = new(
            sin,
            0,
            cos
        );
    }

    private void UpdateFOV()
    {
        ProjectionPlaneWidth = 2f * NearPlaneDepth * (float)Math.Tan(0.5f * MathHelpers.DegToRadCoef * FOV);

        WidthRatio = ResolutionX / ProjectionPlaneWidth;
        HeightRatio = ResolutionY / ProjectionPlaneWidth;
    }

    public void CheckForInput()
    {
        if (InputManager.IsKeyPressedThisFrame[Keyboard.Key.W])
        {
            Position += (InputManager.IsKeyPressedThisFrame[Keyboard.Key.LShift] ? 0.1f : 1f) * (float)TickManager.Delta * 4f * BasisVectorForward;
        }

        if (InputManager.IsKeyPressedThisFrame[Keyboard.Key.S])
        {
            Position -= (InputManager.IsKeyPressedThisFrame[Keyboard.Key.LShift] ? 0.1f : 1f) * (float)TickManager.Delta * 4f * BasisVectorForward;
        }

        if (InputManager.IsKeyPressedThisFrame[Keyboard.Key.A])
        {
            Position += (InputManager.IsKeyPressedThisFrame[Keyboard.Key.LShift] ? 0.1f : 1f) * (float)TickManager.Delta * 4f * new Vector3(-BasisVectorForward.Z, 0, BasisVectorForward.X);
        }

        if (InputManager.IsKeyPressedThisFrame[Keyboard.Key.D])
        {
            Position -= (InputManager.IsKeyPressedThisFrame[Keyboard.Key.LShift] ? 0.1f : 1f) * (float)TickManager.Delta * 4f * new Vector3(-BasisVectorForward.Z, 0, BasisVectorForward.X);
        }

        if (InputManager.IsKeyPressedThisFrame[Keyboard.Key.Left])
        {
            Rotation += (InputManager.IsKeyPressedThisFrame[Keyboard.Key.LShift] ? 0.1f : 1f) * (float)TickManager.Delta * new Vector3(0, 90, 0);

            UpdateBasisVectorForward();
        }

        if (InputManager.IsKeyPressedThisFrame[Keyboard.Key.Right])
        {
            Rotation += (InputManager.IsKeyPressedThisFrame[Keyboard.Key.LShift] ? 0.1f : 1f) * (float)TickManager.Delta * new Vector3(0, -90, 0);

            UpdateBasisVectorForward();
        }

        if (InputManager.IsKeyPressedThisFrame[Keyboard.Key.I])
        {
            FOV -= 20f * (float)TickManager.Delta;
            UpdateFOV();
        }

        if (InputManager.IsKeyPressedThisFrame[Keyboard.Key.O])
        {
            FOV += 20f * (float)TickManager.Delta;
            UpdateFOV();
        }

        if (InputManager.IsKeyImpulseThisFrame[Keyboard.Key.B])
        {
            IsDebugRender = !IsDebugRender;
        }
    }
}