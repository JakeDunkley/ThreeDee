using System.Drawing;
using System.Numerics;

namespace ThreeDee;

public class Scene
{
    public SceneCamera Camera;
    public List<SceneObject> SceneObjects;

    public Scene()
    {
        Camera = new();
        SceneObjects = new();
    }

    public void Render()
    {
        Camera.ClearBuffers();

        foreach (SceneObject obj in SceneObjects)
        {
            obj.Transform();
            obj.ProjectToScreenSpace(Camera);

            for (int i = 0; i < obj.Triangles.Length; i++)
            {
                int[] bounds = obj.CalculatePixelScreenSpaceBounds(obj.Triangles[i], Camera);

                for (int row = bounds[1]; row < bounds[3]; row++)
                {
                    for (int col = bounds[0]; col < bounds[2]; col++)
                    {
                        Vector3 ssPoint = new(col, row, 0);

                        // Camera.ColorBuffer[row, col] = Structs.Color.Red;

                        if (MathHelpers.IsPointInside(ssPoint, obj.ScreenSpaceVertices[obj.Triangles[i].A], obj.ScreenSpaceVertices[obj.Triangles[i].B], obj.ScreenSpaceVertices[obj.Triangles[i].C]))
                        {
                            float depth = MathHelpers.CalculateDepth(ssPoint, obj.ScreenSpaceVertices[obj.Triangles[i].A], obj.ScreenSpaceVertices[obj.Triangles[i].B], obj.ScreenSpaceVertices[obj.Triangles[i].C]);

                            if (Camera.DepthBuffer[row, col] == 0f || Camera.DepthBuffer[row, col] > depth)
                            {
                                Camera.DepthBuffer[row, col] = depth;
                                Camera.ColorBuffer[row, col] = (1f - depth) * Structs.Color.Green;
                            }
                        }
                    }
                }
            }
        }
    }
}