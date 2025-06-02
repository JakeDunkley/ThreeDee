using SFML.System;
using SFML.Graphics;
using ThreeDee.Structs;
using SFML.Window;
using System.Numerics;

namespace ThreeDee;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Starting.....");

        WindowManager.Window.SetActive();

        Vector3 a = new(-0.35f, -0.25f, 1.0f);
        Vector3 b = new(-0.35f,  0.25f, 1.0f);
        Vector3 c = new( 0.15f, -0.25f, 1.0f);
        Vector3 d = new(-0.35f, -0.25f, 2.0f);
        Vector3 e = new(-0.35f,  0.25f, 2.0f);
        Vector3 f = new( 0.15f, -0.25f, 2.0f);
        Vector3 g = new(-0.35f, -0.25f, 3.0f);
        Vector3 h = new(-0.35f,  0.25f, 3.0f);
        Vector3 i = new( 0.15f, -0.25f, 3.0f);

        Triangle3D t1a = new(a, b, c);
        Triangle3D t2a = new(d, e, f);
        Triangle3D t3a = new(g, h, i);

        Triangle2D t1b = t1a.ProjectToScreenSpace(1f, 1f, 1f, WindowManager.WindowSizeX, WindowManager.WindowSizeY);
        Triangle2D t2b = t2a.ProjectToScreenSpace(1f, 1f, 1f, WindowManager.WindowSizeX, WindowManager.WindowSizeY);
        Triangle2D t3b = t3a.ProjectToScreenSpace(1f, 1f, 1f, WindowManager.WindowSizeX, WindowManager.WindowSizeY);

        while (WindowManager.WindowIsOpen)
        {
            TickManager.FrameStart();

            WindowManager.DispatchEvents();
            WindowManager.ClearWindow();

            // Your frame rendering code here.....
            t3b.Render(Structs.Color.Red);
            t2b.Render(Structs.Color.Green);
            t1b.Render(Structs.Color.Blue);

            WindowManager.Draw();

            WindowManager.DisplayWindow();
            WindowManager.CheckIfShouldClose();

            TickManager.FrameEnd();
            Console.WriteLine(TickManager.Debug_FrameTime());
        }
    }
}
