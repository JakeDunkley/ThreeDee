using System.Numerics;
using SFML.Graphics;
using ThreeDee.Structs;

namespace ThreeDee;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Starting.....");

        WindowManager.Window.SetActive();

        SceneObject cube = new("../../../models/cube.obj")
        {
            Translation = new(0, 0, 3),
            Shader = new TextureShader("../../../textures/colorGrid.png")
        };

        SceneObject suzanne = new("../../../models/suzanne.obj")
        {
            Translation = new(3, 0, 3),
            Shader = new RandomTriColorShader()
        };

        Text debugText = OverlayManager.CreateAndAddOverlayTextObject("");
        debugText.Scale = new(0.5f, 0.5f);

        Scene scene = new();
        scene.SceneObjects.Add(cube);
        scene.SceneObjects.Add(suzanne);

        while (WindowManager.WindowIsOpen)
        {
            TickManager.FrameStart();

            WindowManager.DispatchEvents();
            InputManager.PollInput();
            // WindowManager.InducePain();

            cube.RotateBy(new Vector3(0, (float)(90.0 * TickManager.Delta), 0));

            scene.RenderParallel();

            WindowManager.DrawParallel(scene);
            OverlayManager.DrawOverlay();

            WindowManager.DisplayWindow();
            WindowManager.CheckForInput();

            TickManager.FrameEnd();
            debugText.DisplayedString = TickManager.FrameTimeInfo();
        }
    }
}
