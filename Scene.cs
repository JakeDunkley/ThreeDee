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

    public void RenderParallel()
    {
        Camera.ClearBuffers();
        Camera.CheckForInput();

        if (Camera.IsDebugRender)
        {
            Render_Debug();
        }

        else
        {
            RenderMainParallel();
        }
    }

    public void RenderMainParallel()
    {
        foreach (SceneObject obj in SceneObjects)
        {
            obj.TransformParallel(Camera);
            obj.ProjectToScreenSpaceParallel(Camera);

            for (int i = 0; i < obj.Triangles.Length; i++)
            {
                int[] bounds = obj.CalculatePixelScreenSpaceBounds(obj.Triangles[i], Camera);

                Parallel.For(bounds[1], bounds[3], row =>
                {
                    for (int col = bounds[0]; col < bounds[2]; col++)
                    {
                        Vector3 ssPoint = new(col, row, 0);

                        Vector3 weights = MathHelpers.CalculateVertexWeights(ssPoint, obj.ScreenSpaceVertices[obj.Triangles[i].A], obj.ScreenSpaceVertices[obj.Triangles[i].B], obj.ScreenSpaceVertices[obj.Triangles[i].C]);

                        if (MathHelpers.IsInside(weights))
                        {
                            float depth = MathHelpers.CalculateDepth(weights, obj.ScreenSpaceVertices[obj.Triangles[i].A], obj.ScreenSpaceVertices[obj.Triangles[i].B], obj.ScreenSpaceVertices[obj.Triangles[i].C]);

                            if (Camera.DepthBuffer[row, col] == 0f || depth < Camera.DepthBuffer[row, col])
                            {
                                Camera.ColorBuffer[row, col] = obj.Shader.ComputeAt(depth, weights, i, obj);
                                Camera.DepthBuffer[row, col] = depth;
                            }
                        }
                    }
                });
            }
        }
    }

    public void Render_Debug()
    {
        foreach (SceneObject obj in SceneObjects)
        {
            obj.TransformParallel(Camera);
            obj.ProjectToScreenSpaceParallel(Camera);

            for (int i = 0; i < obj.Triangles.Length; i++)
            {
                int[] bounds = obj.CalculatePixelScreenSpaceBounds(obj.Triangles[i], Camera);

                Parallel.For(bounds[1], bounds[3], row =>
                {
                    for (int col = bounds[0]; col < bounds[2]; col++)
                    {
                        Vector3 ssPoint = new(col, row, 0);

                        Vector3 weights = MathHelpers.CalculateVertexWeights(ssPoint, obj.ScreenSpaceVertices[obj.Triangles[i].A], obj.ScreenSpaceVertices[obj.Triangles[i].B], obj.ScreenSpaceVertices[obj.Triangles[i].C]);

                        if (MathHelpers.IsInside(weights))
                        {
                            float depth = MathHelpers.CalculateDepth(weights, obj.ScreenSpaceVertices[obj.Triangles[i].A], obj.ScreenSpaceVertices[obj.Triangles[i].B], obj.ScreenSpaceVertices[obj.Triangles[i].C]);
                            Vector3 normal = MathHelpers.CalculateNormalSmooth(weights, obj.Triangles[i], obj);

                            if (Camera.DepthBuffer[row, col] == 0f || depth < Camera.DepthBuffer[row, col])
                            {
                                Structs.Color normalColor = 0.5f * (new Structs.Color(normal.X, normal.Y, normal.Z) + 1f);
                                Camera.ColorBuffer[row, col] = normalColor;
                                Camera.DepthBuffer[row, col] = depth;
                            }
                        }
                    }
                });
            }
        }
    }
}