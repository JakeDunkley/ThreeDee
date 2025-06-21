using System.Numerics;
using ThreeDee.Structs;

namespace ThreeDee;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Starting.....");

        WindowManager.Window.SetActive();

        SceneObject test = new("../../../models/cube.obj")
        {
            // Rotation = new Vector3(30, 0, 0),
            Translation = new Vector3(0, 0, 4)
        };

        Scene scene = new();
        scene.SceneObjects.Add(test);


        while (WindowManager.WindowIsOpen)
        {
            TickManager.FrameStart();

            WindowManager.DispatchEvents();
            WindowManager.ClearWindow();

            test.RotateBy(new Vector3(0, 1, 0));

            scene.RenderParallel();

            WindowManager.DrawParallel(scene);

            WindowManager.DisplayWindow();
            WindowManager.CheckIfShouldClose();

            TickManager.FrameEnd();
            Console.WriteLine(TickManager.Debug_FrameTimeInfo());
        }
    }
}
