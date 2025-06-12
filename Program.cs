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

        Model head = new("../../../models/suzanne_head.obj");
        Model eyeLeft = new("../../../models/suzanne_eye_left.obj");
        Model eyeRight = new("../../../models/suzanne_eye_right.obj");

        RenderManager.MaterialTest();


        while (WindowManager.WindowIsOpen)
        {
            TickManager.FrameStart();

            WindowManager.DispatchEvents();
            WindowManager.ClearWindow();

            // model.RotateY(1f);
            // model.Translate(new Vector3(0, 0, 4));
            head.RotateY(1f);
            eyeLeft.RotateY(1f);
            eyeRight.RotateY(1f);
            head.Translate(new Vector3(0, 0, 3));
            eyeLeft.Translate(new Vector3(0, 0, 3));
            eyeRight.Translate(new Vector3(0, 0, 3));

            RenderManager.RenderTest();

            // model.Translate(new Vector3(0, 0, -4));
            head.Translate(new Vector3(0, 0, -3));
            eyeLeft.Translate(new Vector3(0, 0, -3));
            eyeRight.Translate(new Vector3(0, 0, -3));

            WindowManager.Draw();

            WindowManager.DisplayWindow();
            WindowManager.CheckIfShouldClose();

            TickManager.FrameEnd();
            Console.WriteLine(TickManager.Debug_FrameTimeInfo());
        }
    }
}
