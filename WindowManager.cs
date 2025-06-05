using SFML.Graphics;
using SFML.Window;

namespace ThreeDee;

public static class WindowManager
{
    public const int WindowSizeX = 512;
    public const int WindowSizeY = 512;
    private static readonly string _windowTitle = "ThreeDee!";

    public static readonly RenderWindow Window = new(new VideoMode(WindowSizeX, WindowSizeY), _windowTitle);
    public static readonly RenderStates RenderStates = new();
    public static readonly Texture WindowTexture = new(WindowSizeX, WindowSizeY);
    public static readonly Sprite WindowSprite = new(WindowTexture);

    public static readonly Structs.Color[,] PixelGrid = InitializePixelGrid();

    public static bool WindowIsOpen => Window.IsOpen;

    public static void ClearWindow()
    {
        for (int row = 0; row < WindowSizeY; row++)
        {
            for (int col = 0; col < WindowSizeX; col++)
            {
                PixelGrid[row, col] = Structs.Color.Black;
            }
        }
    }

    public static void Draw()
    {
        WindowTexture.Update(ConvertPixelGridToByteArray());
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

    private static Structs.Color[,] InitializePixelGrid()
    {
        Structs.Color[,] pixelGrid = new Structs.Color[WindowSizeY, WindowSizeX];

        for (int row = 0; row < WindowSizeY; row++)
        {
            for (int col = 0; col < WindowSizeX; col++)
            {
                pixelGrid[row, col] = Structs.Color.Black;
            }
        }

        return pixelGrid;
    }

    public static byte[] ConvertPixelGridToByteArray()
    {
        byte[] bytes = new byte[WindowSizeY * WindowSizeX * 4];

        for (int row = 0; row < WindowSizeY; row++)
        {
            for (int col = 0; col < WindowSizeX; col++)
            {
                int pixelCoordinate = ((WindowSizeY - row - 1) * WindowSizeY + col) * 4;

                bytes[pixelCoordinate]     = PixelGrid[row, col].ByteR;
                bytes[pixelCoordinate + 1] = PixelGrid[row, col].ByteG;
                bytes[pixelCoordinate + 2] = PixelGrid[row, col].ByteB;
                bytes[pixelCoordinate + 3] = (byte)255;
            }
        }

        return bytes;
    }
}