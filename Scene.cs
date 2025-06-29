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
            obj.ClearProcessedGeometryBuffers();
            obj.TransformParallel(Camera);
            obj.ProjectToScreenSpace(Camera);

            for (int i = 0; i < obj.Triangles.Length; i++)
            {
                if (MathHelpers.IsTriangleOutsideFrustum(obj.Triangles[i], obj, Camera) || MathHelpers.IsTriangleBehindNearPlane(obj.Triangles[i], obj, Camera))
                {
                    continue;
                }

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
                                Vector3 normal = MathHelpers.CalculateNormalSmooth(weights, obj.Triangles[i], obj);
                                float normalizedNormal = 0.5f * (normal.Y + 1f);

                                Camera.ColorBuffer[row, col] = normalizedNormal * obj.Shader.ComputeAt(depth, weights, i, obj);
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
            obj.ClearProcessedGeometryBuffers();

            obj.TransformParallel(Camera);

            for (int i = 0; i < obj.Triangles.Length; i++)
            {
                bool isABehind = MathHelpers.IsVertexBehindNearPlane(obj.TransformedVertices[obj.Triangles[i].A], Camera);
                bool isBBehind = MathHelpers.IsVertexBehindNearPlane(obj.TransformedVertices[obj.Triangles[i].B], Camera);
                bool isCBehind = MathHelpers.IsVertexBehindNearPlane(obj.TransformedVertices[obj.Triangles[i].C], Camera);

                int behindCount = (isABehind ? 1 : 0) + (isBBehind ? 1 : 0) + (isCBehind ? 1 : 0);

                if (behindCount == 0)
                {
                    obj.ClippedTriangles.Add(obj.Triangles[i]);
                    continue;
                }

                if (behindCount == 2)
                {
                    bool isABBehind = isABehind && isBBehind;
                    bool isBCBehind = isBBehind && isCBehind;
                    bool isCABehind = isCBehind && isABehind;

                    MathHelpers.ClipTriangleMissingTwoVertices(isABBehind, isBCBehind, isCABehind, obj.Triangles[i], obj, Camera);
                    continue;
                }

                if (behindCount == 3)
                {
                    continue;
                }
            }

            obj.ProjectToScreenSpace(Camera);

            for (int i = 0; i < obj.ClippedTriangles.Count; i++)
            {
                if (MathHelpers.IsTriangleOutsideFrustum(obj.ClippedTriangles[i], obj, Camera))
                {
                    continue;
                }

                int[] bounds = obj.CalculatePixelScreenSpaceBounds(obj.ClippedTriangles[i], Camera);

                Parallel.For(bounds[1], bounds[3], row =>
                {
                    for (int col = bounds[0]; col < bounds[2]; col++)
                    {
                        Vector3 ssPoint = new(col, row, 0);

                        Vector3 weights = MathHelpers.CalculateVertexWeights(ssPoint, obj.ScreenSpaceVertices[obj.ClippedTriangles[i].A], obj.ScreenSpaceVertices[obj.ClippedTriangles[i].B], obj.ScreenSpaceVertices[obj.ClippedTriangles[i].C]);

                        if (MathHelpers.IsInside(weights))
                        {
                            float depth = MathHelpers.CalculateDepth(weights, obj.ScreenSpaceVertices[obj.ClippedTriangles[i].A], obj.ScreenSpaceVertices[obj.ClippedTriangles[i].B], obj.ScreenSpaceVertices[obj.ClippedTriangles[i].C]);

                            if (Camera.DepthBuffer[row, col] == 0f || depth < Camera.DepthBuffer[row, col])
                            {
                                Vector3 normal = MathHelpers.CalculateNormalSmooth(weights, obj.ClippedTriangles[i], obj);
                                float normalizedNormal = 0.5f * (normal.Y + 1f);

                                Camera.ColorBuffer[row, col] = normalizedNormal * obj.Shader.ComputeAt(depth, weights, i, obj);
                                Camera.DepthBuffer[row, col] = depth;
                            }
                        }
                    }
                });
            }
        }
    }
}