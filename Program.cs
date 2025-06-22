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
            Translation = new(0, 0, 3),
            Shader = new TextureShader("../../../textures/color_grid.png")
        };

        SceneObject suzanne = new("../../../models/suzanne_head.obj")
        {
            Translation = new(3, 0, 3)
        };
        

        Scene scene = new();
        scene.SceneObjects.Add(test);
        scene.SceneObjects.Add(suzanne);


        while (WindowManager.WindowIsOpen)
        {
            TickManager.FrameStart();

            WindowManager.DispatchEvents();
            // WindowManager.ClearWindow();

            test.RotateBy(new Vector3(0, 10f * (float)TickManager.TickDelta, 0));

            scene.RenderParallel();

            WindowManager.DrawParallel(scene);

            WindowManager.DisplayWindow();
            WindowManager.CheckIfShouldClose();

            TickManager.FrameEnd();
            Console.WriteLine(TickManager.Debug_FrameTimeInfo());
        }
    }
}
