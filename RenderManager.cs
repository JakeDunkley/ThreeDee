// using System.Numerics;
// using ThreeDee.Structs;

// namespace ThreeDee;

// public static class RenderManager
// {
//     public static void RenderTest()
//     {
//         ClearDepthBuffer();
//         ProjectWorldVertsToScreenSpace();
//         GenerateScreenSpaceTriangles();

//         for (int i = 0; i < ScreenSpaceTriangleBuffer.Length; i++)
//         {
//             int[] bounds = ScreenSpaceTriangleBuffer[i].CalculatePixelScreenSpaceBounds();

//             Parallel.For(bounds[1], bounds[3], row =>
//             {
//                 for (int col = bounds[0]; col < bounds[2]; col++)
//                 {
//                     Vector3 ssPoint = new(col, row, 0);

//                     // float depth = ScreenSpaceTriangleBuffer[i].CalculateDepth(ssPoint);

//                     // WindowManager.PixelGrid[row, col] = new((depth + 1f) / 5f);

//                     // if (depth >= 0f && depth < DepthBuffer[row, col])
//                     // {
//                     //     DepthBuffer[row, col] = depth;
//                     //     WindowManager.PixelGrid[row, col] = Color.Green;

//                     //     // float dCoef = (depth - 1.5f) / 3f;
//                     //     // WindowManager.PixelGrid[row, col] = (1f - dCoef) * MaterialBuffer[i].CalculateColorAt(ssPoint);
//                     // }

//                     if (ScreenSpaceTriangleBuffer[i].IsPointInside(ssPoint))
//                     {
//                         float depthValue = ScreenSpaceTriangleBuffer[i].CalculateDepthAt(ssPoint);

//                         if (DepthBuffer[row, col] == 0f || DepthBuffer[row, col] > depthValue)
//                         {
//                             DepthBuffer[row, col] = depthValue;
//                             float dCoef = (depthValue - 3.5f) / 3f;
//                             WindowManager.RawPixelGrid[row, col] = (1f - dCoef) * MaterialBuffer[i].CalculateColorAt(ssPoint);
//                         }
//                     }
//                 }
//             });
//         }
//     }
// }