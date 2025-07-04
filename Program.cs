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

        SceneObject ico = new("../../../models/ico.obj", new Shader())
        {
            Translation = new(-3, 0, 3),
        };

        SceneObject cube = new("../../../models/cube.obj", new TextureShader("../../../textures/colorGrid.png"))
        {
            Translation = new(0, 0, 3)
        };

        SceneObject suzanne = new("../../../models/suzanne.obj", new RandomTriColorShader())
        {
            Translation = new(3, 0, 3)
        };

        SceneObject ground = new("../../../models/plane.obj", new TextureShader("../../../textures/colorGrid.png"))
        {
            Translation = new(0, -0.5f, 0),
            Scale = new(10, 1, 10)
        };

        SceneObject flatTri = new("../../../models/flatTri.obj", new TextureShader("../../../textures/colorGrid.png"))
        {
            Translation = new(0, -0.5f, 0),
            Scale = new(2, 1, 2)
        };

        Scene scene = new();
        // scene.SceneObjects.Add(ico);
        // scene.SceneObjects.Add(cube);
        // scene.SceneObjects.Add(suzanne);
        scene.SceneObjects.Add(ground);
        // scene.SceneObjects.Add(flatTri);

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
