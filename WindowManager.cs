using SFML.Graphics;
using SFML.Window;

namespace ThreeDee;

public static class WindowManager
{
    public const int WindowSizeX = 512;
    public const int WindowSizeY = 512;
    private static readonly string _windowTitle = "ThreeDee!";
    private static readonly Color _clearColor = Color.Black;

    public static readonly RenderWindow Window = new(new VideoMode(WindowSizeX, WindowSizeY), _windowTitle);
    public static readonly RenderStates RenderStates = new();

    public static readonly List<List<System.Numerics.Vector3>> PixelGrid = InitializePixelGrid();

    public static void ClearWindow() => Window.Clear(_clearColor);

    public static void DisplayWindow() => Window.Display();

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
                pixelGrid[row].Add(new System.Numerics.Vector3(0f));
            }
        }

        return pixelGrid;
    }
}