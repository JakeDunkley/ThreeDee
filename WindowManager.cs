using SFML.Graphics;
using SFML.Window;

namespace ThreeDee;

public static class WindowManager
{
    public const int RenderSizeX = 256;
    public const int RenderSizeY = 256;
    public const int WindowSizeX = 2 * RenderSizeX;
    public const int WindowSizeY = 2 * RenderSizeY;
    private static readonly string _windowTitle = "ThreeDee!";

    public static readonly RenderWindow Window = new(new VideoMode(WindowSizeX, WindowSizeY), _windowTitle);
    public static readonly RenderStates RenderStates = new();
    public static readonly Texture WindowTexture = new(WindowSizeX, WindowSizeY);
    public static readonly Sprite WindowSprite = new(WindowTexture);

    public static Structs.Color[,] RawPixelGrid = InitializeRawPixelGrid();
    public static Structs.Color[,] UpscaledPixelGrid = new Structs.Color[WindowSizeX, WindowSizeY];

    public static bool WindowIsOpen => Window.IsOpen;

    public static void ClearWindow()
    {
        for (int row = 0; row < RenderSizeY; row++)
        {
            for (int col = 0; col < RenderSizeX; col++)
            {
                RawPixelGrid[row, col] = Structs.Color.Black;
            }
        }
    }

    public static void Draw()
    {
        UpscaleNN();
        WindowTexture.Update(ConvertUpscaledPixelGridToByteArray());
        Window.Draw(WindowSprite);       
    }

    public static void DisplayWindow() => Window.Display();

    public static void DispatchEvents()
    {
        Window.DispatchEvents();
    }

    public static void CheckIfShouldClose()
    {
        if (Keyboard.IsKeyPressed(Keyboard.Key.Escape))
        {
            Window.Close();
        }
    }

    private static Structs.Color[,] InitializeRawPixelGrid()
    {
        Structs.Color[,] pixelGrid = new Structs.Color[RenderSizeY, RenderSizeX];

        for (int row = 0; row < RenderSizeY; row++)
        {
            for (int col = 0; col < RenderSizeX; col++)
            {
                pixelGrid[row, col] = Structs.Color.Black;
            }
        }

        return pixelGrid;
    }

    private static void UpscaleNN()
    {
        Parallel.For(0, RenderSizeY, row =>
        {
            for (int col = 0; col < RenderSizeX; col++)
            {
                UpscaledPixelGrid[2 * row, 2 * col] = RawPixelGrid[row, col];
                UpscaledPixelGrid[(2 * row) + 1, 2 * col] = RawPixelGrid[row, col];
                UpscaledPixelGrid[2 * row, (2 * col) + 1] = RawPixelGrid[row, col];
                UpscaledPixelGrid[(2 * row) + 1, (2 * col) + 1] = RawPixelGrid[row, col];
            }
        });
    }

    private static void UpscaleBilinear()
    {
        for (int row = 0; row < RenderSizeY - 2; row++)
        {
            for (int col = 0; col < RenderSizeX - 2; col++)
            {
                UpscaledPixelGrid[2 * row, 2 * col] = RawPixelGrid[row, col];
                UpscaledPixelGrid[(2 * row) + 1, 2 * col] = 0.5f * (RawPixelGrid[row, col] + RawPixelGrid[row + 1, col]);
                UpscaledPixelGrid[2 * row, (2 * col) + 1] = 0.5f * (RawPixelGrid[row, col] + RawPixelGrid[row, col + 1]);
                UpscaledPixelGrid[(2 * row) + 1, (2 * col) + 1] = 0.25f * (RawPixelGrid[row, col] + RawPixelGrid[row + 1, col] + RawPixelGrid[row, col + 1] + RawPixelGrid[row + 1, col + 1]);
            }
        }
    }

    private static void UpscaleAA()
    {
        for (int row = 0; row < RenderSizeY - 1; row++)
        {
            for (int col = 0; col < RenderSizeX - 1; col++)
            {
                float dx = float.Abs(RawPixelGrid[row, col + 1].Luminance() - RawPixelGrid[row, col].Luminance());
                float dy = float.Abs(RawPixelGrid[row + 1, col].Luminance() - RawPixelGrid[row, col].Luminance());
                float dm = (dx + dy) * 0.70710678f;

                // float sdx = dx * dx * (3f - (2f * dx)); // smoothstep
                // float sdy = dy * dy * (3f - (2f * dy)); // smoothstep
                // float sdm = dm * dm * (3f - (2f * dm)); // smoothstep

                // UpscaledPixelGrid[2 * row, 2 * col] = new(dx, dy, 0);
                // UpscaledPixelGrid[(2 * row) + 1, 2 * col] = new(dx, dy, 0);
                // UpscaledPixelGrid[2 * row, (2 * col) + 1] = new(dx, dy, 0);
                // UpscaledPixelGrid[(2 * row) + 1, (2 * col) + 1] = new(dx, dy, 0);
                UpscaledPixelGrid[2 * row, 2 * col] = RawPixelGrid[row, col].Luminance() * Structs.Color.White;
                UpscaledPixelGrid[(2 * row) + 1, 2 * col] = Structs.Color.Green;
                UpscaledPixelGrid[2 * row, (2 * col) + 1] = Structs.Color.Red;
                UpscaledPixelGrid[(2 * row) + 1, (2 * col) + 1] = Structs.Color.Blue;

                // UpscaledPixelGrid[2 * row, 2 * col] = RawPixelGrid[row, col];
                // UpscaledPixelGrid[(2 * row) + 1, 2 * col] = (sdy * RawPixelGrid[row, col]) + ((1f - sdy) * RawPixelGrid[row + 1, col]);
                // UpscaledPixelGrid[2 * row, (2 * col) + 1] = (sdx * RawPixelGrid[row, col]) + ((1f - sdx) * RawPixelGrid[row, col + 1]);
                // UpscaledPixelGrid[(2 * row) + 1, (2 * col) + 1] =(sdm * RawPixelGrid[row, col]) + ((1f - sdm) * RawPixelGrid[row + 1, col + 1]);
            }
        }
    }

    public static byte[] ConvertUpscaledPixelGridToByteArray()
    {
        byte[] bytes = new byte[WindowSizeY * WindowSizeX * 4];

        for (int row = 0; row < WindowSizeY; row++)
        {
            for (int col = 0; col < WindowSizeX; col++)
            {
                int pixelCoordinate = ((WindowSizeY - row - 1) * WindowSizeY + col) * 4;

                bytes[pixelCoordinate] = UpscaledPixelGrid[row, col].ByteR;
                bytes[pixelCoordinate + 1] = UpscaledPixelGrid[row, col].ByteG;
                bytes[pixelCoordinate + 2] = UpscaledPixelGrid[row, col].ByteB;
                bytes[pixelCoordinate + 3] = (byte)255;
            }
        }

        return bytes;
    }
}