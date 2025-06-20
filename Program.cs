using System.Numerics;
using ThreeDee.Structs;

namespace ThreeDee;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Starting.....");

        WindowManager.Window.SetActive();

        // Model model = new("../../../models/rotTest.obj");
        // model.Scale(new Vector3(1));
        // model.RotateX(45);

        // Model head = new("../../../models/suzanne_head.obj");
        Model head = new("../../../models/dragon.obj");
        head.RotateX(30f);

        RenderManager.MaterialTest();


        while (WindowManager.WindowIsOpen)
        {
            TickManager.FrameStart();

            WindowManager.DispatchEvents();
            WindowManager.ClearWindow();

            head.RotateY(1f);
            head.Translate(new Vector3(0, 0, 1.25f));
            // head.Translate(new Vector3(0, 0, 5));

            RenderManager.RenderTest();

            head.Translate(new Vector3(0, 0, -1.25f));
            // head.Translate(new Vector3(0, 0, -5));

            WindowManager.Draw();

            WindowManager.DisplayWindow();
            WindowManager.CheckIfShouldClose();

            TickManager.FrameEnd();
            Console.WriteLine(TickManager.Debug_FrameTimeInfo());
        }
    }
}
