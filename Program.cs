using System.Numerics;
using ThreeDee.Structs;

namespace ThreeDee;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Starting.....");

        WindowManager.Window.SetActive();

        Model model = Model.FromWavefrontFile("../../../models/icoHemi.obj");
        model.Scale(new Vector3(1));
        model.Translate(new Vector3(0, 0, 4));

        while (WindowManager.WindowIsOpen)
        {
            TickManager.FrameStart();

            WindowManager.DispatchEvents();
            WindowManager.ClearWindow();

            float x = (float)Math.Cos(TickManager.SecondsElapse * 10) * 0.05f;
            float y = (float)Math.Sin(TickManager.SecondsElapse * 10) * 0.05f;

            model.Translate(new Vector3(x, y, 0));

            model.Render();

            WindowManager.Draw();

            WindowManager.DisplayWindow();
            WindowManager.CheckIfShouldClose();

            TickManager.FrameEnd();
            Console.WriteLine(TickManager.Debug_FrameTimeInfo());
        }
    }
}
