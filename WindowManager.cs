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

    public static readonly List<List<System.Numerics.Vector3>> PixelGrid = InitializePixelGrid();

    public static bool WindowIsOpen => Window.IsOpen;

    public static void ClearWindow()
    {
        foreach (List<System.Numerics.Vector3> row in PixelGrid)
        {
            for (int col = 0; col < WindowSizeY; col++)
            {
                row[col] = System.Numerics.Vector3.Zero;
            }
        }
    }

    public static void DisplayWindow() => Window.Display();

    public static void UpdateWindow()
    {
        CheckIfShouldClose();
        Window.WaitAndDispatchEvents();
    }

    public static void CheckIfShouldClose()
    {
        if (Keyboard.IsKeyPressed(Keyboard.Key.Escape))
        {
            Window.Close();
        }
    }

    private static List<List<System.Numerics.Vector3>> InitializePixelGrid()
    {
        List<List<System.Numerics.Vector3>> pixelGrid = [];

        for (int row = 0; row < WindowSizeY; row++)
        {
            pixelGrid.Add([]);

            for (int col = 0; col < WindowSizeX; col++)
            {
                pixelGrid[row].Add(System.Numerics.Vector3.Zero);
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

                bytes[pixelCoordinate]     = (byte)(255 * PixelGrid[row][col].X);
                bytes[pixelCoordinate + 1] = (byte)(255 * PixelGrid[row][col].Y);
                bytes[pixelCoordinate + 2] = (byte)(255 * PixelGrid[row][col].Z);
                bytes[pixelCoordinate + 3] = (byte)255;
            }
        }

        return bytes;
    }
}