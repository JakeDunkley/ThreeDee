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

        SceneObject ico = new("../../../models/ico.obj")
        {
            Translation = new(-3, 0, 3),
        };

        SceneObject cube = new("../../../models/cube.obj")
        {
            Translation = new(0, 0, 3),
            // Rotation = new(-15, 0, 0),
            Shader = new TextureShader("../../../textures/colorGrid.png")
        };

        SceneObject suzanne = new("../../../models/suzanne.obj")
        {
            Translation = new(3, 0, 3),
            Shader = new RandomTriColorShader()
        };

        SceneObject ground = new("../../../models/plane.obj")
        {
            Translation = new(0, -1, 0),
            Scale = new(10, 1, 10),
            Shader = new RandomTriColorShader()
        };

        Text debugText = OverlayManager.CreateAndAddOverlayTextObject("");
        debugText.Scale = new(0.5f, 0.5f);

        Scene scene = new();
        scene.SceneObjects.Add(ico);
        scene.SceneObjects.Add(cube);
        scene.SceneObjects.Add(suzanne);

        while (WindowManager.WindowIsOpen)
        {
            TickManager.FrameStart();

            WindowManager.DispatchEvents();
            InputManager.PollInput();
            // WindowManager.InducePain();

            // cube.RotateBy(new Vector3(0, (float)(90.0 * TickManager.Delta), 0));
            suzanne.RotateBy(new Vector3(0, 0, (float)(90.0 * TickManager.Delta)));

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
