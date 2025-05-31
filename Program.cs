using SFML.System;
using SFML.Graphics;
using ThreeDee.Structs;
using SFML.Window;

namespace ThreeDee;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Starting.....");

        System.Numerics.Vector3 a = new(20, 20, 0f);
        System.Numerics.Vector3 b = new(256, 492, 0f);
        System.Numerics.Vector3 c = new(492, 20, 0f);

        Triangle triangle = new(a, b, c);

        for (int row = 0; row < WindowManager.WindowSizeY; row++)
        {
            for (int col = 0; col < WindowManager.WindowSizeX; col++)
            {
                System.Numerics.Vector3 coord = new System.Numerics.Vector3(col, row, 0f);

                if (triangle.IsPointInside(coord))
                {
                    WindowManager.PixelGrid[row][col] = new System.Numerics.Vector3((float)col / WindowManager.WindowSizeX, (float)row / WindowManager.WindowSizeY, 0f);
                }

                if (a.X == coord.X && a.Y == coord.Y)
                {
                    WindowManager.PixelGrid[row][col] = new System.Numerics.Vector3(0f, 1f, 1f);
                }

                if (b.X == coord.X && b.Y == coord.Y)
                {
                    WindowManager.PixelGrid[row][col] = new System.Numerics.Vector3(1f, 0f, 1f);
                }

                if (c.X == coord.X && c.Y == coord.Y)
                {
                    WindowManager.PixelGrid[row][col] = new System.Numerics.Vector3(1f, 1f, 0f);
                }
            }
        }

        Helpers.WriteImageDataToFile(WindowManager.PixelGrid, "triangleTest");

        // while (WindowManager.Window.IsOpen)
        // {
        //     TickManager.FrameStart();

        //     WindowManager.ClearWindow();

        //     // Your frame rendering code here.....

        //     WindowManager.DisplayWindow();
        //     WindowManager.CheckIfShouldClose();

        //     TickManager.FrameEnd();
        // }
    }
}