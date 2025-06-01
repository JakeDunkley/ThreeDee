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

        System.Numerics.Vector3 a =  new(-0.25f, -0.25f, 1.0f);
        System.Numerics.Vector3 b =  new(-0.25f,  0.25f, 1.0f);
        System.Numerics.Vector3 c =  new( 0.25f, -0.25f, 1.0f);
        System.Numerics.Vector3 b2 = new(-0.25f,  0.25f, 2.0f);
        System.Numerics.Vector3 c2 = new( 0.25f, -0.25f, 2.0f);
        System.Numerics.Vector3 d =  new( 0.25f,  0.25f, 2.0f);

        Triangle3D t3D1 = new(a, b, c);
        Triangle3D t3D2 = new(c2, b2, d);

        Triangle2D t2D1 = t3D1.ProjectToScreenSpace(1f, 1f, 1f, WindowManager.WindowSizeX, WindowManager.WindowSizeY);
        Triangle2D t2D2 = t3D2.ProjectToScreenSpace(1f, 1f, 1f, WindowManager.WindowSizeX, WindowManager.WindowSizeY);

        t2D1.Render(new System.Numerics.Vector3(0.33f, 0.00f, 0.67f));
        t2D2.Render(new System.Numerics.Vector3(0.50f, 0.67f, 0.13f));

        Helpers.WriteImageDataToFile(WindowManager.PixelGrid, "square");

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