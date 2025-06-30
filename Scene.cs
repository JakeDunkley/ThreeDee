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
                obj.ClipTriangles.Add(obj.Triangles[i]);

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

                        Vector3 weights = MathHelpers.CalculateVertexWeights(ssPoint, obj.SSVertices[obj.Triangles[i].A], obj.SSVertices[obj.Triangles[i].B], obj.SSVertices[obj.Triangles[i].C]);

                        if (MathHelpers.IsInside(weights))
                        {
                            float depth = MathHelpers.CalculateDepth(weights, obj.SSVertices[obj.Triangles[i].A], obj.SSVertices[obj.Triangles[i].B], obj.SSVertices[obj.Triangles[i].C]);

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
                // bool isABehind = MathHelpers.IsVertexBehindNearPlane(obj.TransformedVertices[obj.Triangles[i].A], Camera);
                // bool isBBehind = MathHelpers.IsVertexBehindNearPlane(obj.TransformedVertices[obj.Triangles[i].B], Camera);
                // bool isCBehind = MathHelpers.IsVertexBehindNearPlane(obj.TransformedVertices[obj.Triangles[i].C], Camera);
                bool isABehind = obj.TransVertices[obj.Triangles[i].A].Z <= Camera.NearPlaneDepth;
                bool isBBehind = obj.TransVertices[obj.Triangles[i].B].Z <= Camera.NearPlaneDepth;
                bool isCBehind = obj.TransVertices[obj.Triangles[i].C].Z <= Camera.NearPlaneDepth;

                int behindCount = (isABehind ? 1 : 0) + (isBBehind ? 1 : 0) + (isCBehind ? 1 : 0);

                if (behindCount == 0)
                {
                    obj.ClipTriangles.Add(obj.Triangles[i]);
                    continue;
                }

                if (behindCount == 1)
                {
                    MathHelpers.ClipTriangleMissingOneVertex(isABehind, isBBehind, isCBehind, obj.Triangles[i], obj, Camera);
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

            for (int i = 0; i < obj.ClipTriangles.Count; i++)
            {
                if (MathHelpers.IsTriangleOutsideFrustum(obj.ClipTriangles[i], obj, Camera))
                {
                    continue;
                }

                int[] bounds = obj.CalculatePixelScreenSpaceBounds(obj.ClipTriangles[i], Camera);

                Parallel.For(bounds[1], bounds[3], row =>
                {
                    for (int col = bounds[0]; col < bounds[2]; col++)
                    {
                        Vector3 ssPoint = new(col, row, 0);

                        Vector3 weights = MathHelpers.CalculateVertexWeights(ssPoint, obj.SSVertices[obj.ClipTriangles[i].A], obj.SSVertices[obj.ClipTriangles[i].B], obj.SSVertices[obj.ClipTriangles[i].C]);

                        if (MathHelpers.IsInside(weights))
                        {
                            float depth = MathHelpers.CalculateDepth(weights, obj.SSVertices[obj.ClipTriangles[i].A], obj.SSVertices[obj.ClipTriangles[i].B], obj.SSVertices[obj.ClipTriangles[i].C]);

                            if (Camera.DepthBuffer[row, col] == 0f || depth < Camera.DepthBuffer[row, col])
                            {
                                Vector3 normal = MathHelpers.CalculateNormalSmooth(weights, obj.ClipTriangles[i], obj);
                                float normalizedNormal = 0.5f * (normal.Y + 1f);

                                // Camera.ColorBuffer[row, col] = new Structs.Color(normal.X * 0.5f + 0.5f, normal.Y * 0.5f + 0.5f, normal.Z * 0.5f + 0.5f);
                                Camera.ColorBuffer[row, col] = normalizedNormal * obj.Shader.ComputeAt(depth, weights, i, obj);
                                Structs.Color depthColor;

                                if (depth > 0f && depth < 1f)
                                {
                                    depthColor = depth * Structs.Color.Green;
                                }

                                else if (depth <= 0f)
                                {
                                    depthColor = Structs.Color.Red;
                                }

                                else
                                {
                                    depthColor = Structs.Color.Cyan;
                                }
                                Camera.ColorBuffer[row, col] = depthColor;

                                Camera.DepthBuffer[row, col] = depth;
                            }
                        }
                    }
                });
            }
        }
    }
}