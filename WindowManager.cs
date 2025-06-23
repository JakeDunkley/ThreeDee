using SFML.Graphics;
using SFML.Window;

namespace ThreeDee;

public static class WindowManager
{
    public const int WindowX = 1024;
    public const int WindowY = 1024;
    private static readonly string _windowTitle = "ThreeDee!";

    public static readonly RenderWindow Window = new(new VideoMode(WindowX, WindowY), _windowTitle);
    public static readonly RenderStates RenderStates = new();
    public static readonly Texture WindowTexture = new(WindowX, WindowY);
    public static readonly Sprite WindowSprite = new(WindowTexture);
    public static readonly Structs.Color[,] ColorBuffer = new Structs.Color[WindowX, WindowY];

    public static bool WindowIsOpen => Window.IsOpen;

    public static void InducePain()
    {
        for (int row = 0; row < WindowY; row++)
        {
            for (int col = 0; col < WindowX; col++)
            {
                ColorBuffer[row, col] = Structs.Color.Black;
            }
        }
    }

    public static void Draw(Scene scene)
    {
        UpscaleNN(scene.Camera.ColorBuffer);
        WindowTexture.Update(ConvertColorBufferToByteArray());
        Window.Draw(WindowSprite);
    }

    public static void DrawParallel(Scene scene)
    {
        UpscaleNNParallel(scene.Camera.ColorBuffer);
        WindowTexture.Update(ConvertColorBufferToByteArrayParallel());
        Window.Draw(WindowSprite);
    }

    public static void DisplayWindow() => Window.Display();

    public static void DispatchEvents()
    {
        Window.DispatchEvents();
    }

    public static void CheckForInput()
    {
        if (InputManager.IsKeyImpulseThisFrame[Keyboard.Key.Escape])
        {
            Window.Close();
        }

        if (InputManager.IsKeyImpulseThisFrame[Keyboard.Key.P])
        {
            Image capture = WindowTexture.CopyToImage();
            capture.SaveToFile($"../../../out/capture_{DateTime.Now:yyyy-MM-dd_HH.mm.ss}.png");
        }
    }
    private static void UpscaleNN(Structs.Color[,] rawColorBuffer)
    {
        float invUpscaleRatioX = (float)rawColorBuffer.GetLength(1) / WindowX;
        float invUpscaleRatioY = (float)rawColorBuffer.GetLength(0) / WindowY;

        for (int row = 0; row < WindowY; row++)
        {
            for (int col = 0; col < WindowX; col++)
            {
                ColorBuffer[row, col] = rawColorBuffer[(int)(row * invUpscaleRatioY), (int)(col * invUpscaleRatioX)];
            }
        }
    }

    private static void UpscaleNNParallel(Structs.Color[,] rawColorBuffer)
    {
        float invUpscaleRatioX = (float)rawColorBuffer.GetLength(1) / WindowX;
        float invUpscaleRatioY = (float)rawColorBuffer.GetLength(0) / WindowY;

        Parallel.For(0, WindowY, row =>
        {
            for (int col = 0; col < WindowX; col++)
            {
                ColorBuffer[row, col] = rawColorBuffer[(int)(row * invUpscaleRatioY), (int)(col * invUpscaleRatioX)];
            }
        });
    }

    public static byte[] ConvertColorBufferToByteArray()
    {
        byte[] bytes = new byte[WindowY * WindowX * 4];

        for (int row = 0; row < WindowY; row++)
        {
            for (int col = 0; col < WindowX; col++)
            {
                int pixelCoordinate = ((WindowY - row - 1) * WindowY + col) * 4;

                bytes[pixelCoordinate] = ColorBuffer[row, col].ByteR;
                bytes[pixelCoordinate + 1] = ColorBuffer[row, col].ByteG;
                bytes[pixelCoordinate + 2] = ColorBuffer[row, col].ByteB;
                bytes[pixelCoordinate + 3] = 255;
            }
        }

        return bytes;
    }
    
    public static byte[] ConvertColorBufferToByteArrayParallel()
    {
        byte[] bytes = new byte[WindowY * WindowX * 4];

        Parallel.For(0, WindowY, row =>
        {
            for (int col = 0; col < WindowX; col++)
            {
                int pixelCoordinate = ((WindowY - row - 1) * WindowY + col) * 4;

                bytes[pixelCoordinate] = ColorBuffer[row, col].ByteR;
                bytes[pixelCoordinate + 1] = ColorBuffer[row, col].ByteG;
                bytes[pixelCoordinate + 2] = ColorBuffer[row, col].ByteB;
                bytes[pixelCoordinate + 3] = 255;
            }
        });

        return bytes;
    }
}