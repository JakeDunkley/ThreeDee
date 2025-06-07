using System.Numerics;
using ThreeDee.Structs;

namespace ThreeDee;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Starting.....");

        WindowManager.Window.SetActive();

        Model model = new("../../../models/cube.obj");
        model.Scale(new Vector3(1));
        model.Translate(new Vector3(0, 0, 2));

        while (WindowManager.WindowIsOpen)
        {
            TickManager.FrameStart();

            WindowManager.DispatchEvents();
            WindowManager.ClearWindow();

            RenderManager.RenderTest();

            WindowManager.Draw();

            WindowManager.DisplayWindow();
            WindowManager.CheckIfShouldClose();

            TickManager.FrameEnd();
            Console.WriteLine(TickManager.Debug_FrameTimeInfo());
        }
    }
}
