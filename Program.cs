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
            // Shader = new RandomTriColorShader()
            Shader = new TextureShader("../../../textures/colorGrid.png")
        };

        Scene scene = new();
        // scene.SceneObjects.Add(ico);
        // scene.SceneObjects.Add(cube);
        // scene.SceneObjects.Add(suzanne);
        scene.SceneObjects.Add(ground);

        Text frameTimeText = OverlayManager.CreateAndAddOverlayTextObject("");
        frameTimeText.Scale = new(0.5f, 0.5f);

        Text debugText = OverlayManager.CreateAndAddOverlayTextObject("");
        debugText.Scale = new(0.5f, 0.5f);
        debugText.Position = new(WindowManager.WindowX - 50f, 0f);

        while (WindowManager.WindowIsOpen)
        {
            TickManager.FrameStart();

            WindowManager.DispatchEvents();
            InputManager.PollInput();

            suzanne.RotateBy(new Vector3(0, 0, (float)(90.0 * TickManager.Delta)));

            scene.RenderParallel();

            WindowManager.DrawParallel(scene);
            OverlayManager.DrawOverlay();

            WindowManager.DisplayWindow();
            WindowManager.CheckForInput();

            TickManager.FrameEnd();
            frameTimeText.DisplayedString = TickManager.FrameTimeInfo();
            debugText.DisplayedString = scene.Camera.IsDebugRender ? "DEBUG" : "";
        }
    }
}
