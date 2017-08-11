using System;
using UnityEngine;

namespace Common.Grid.Physics
{
    [Flags]
    public enum ETileSurfaceSimpleBlockerType
    {
        None = 0,
        BlockerTop = 1,
        BlockerBottom = 2,
        BlockerTopAndBottom = BlockerTop | BlockerBottom,

        BitStride = 2
    }

    /// <summary>
    /// Available settings for tile blocker height and other options.
    /// </summary>
    [Flags]
    public enum ETileEdgeSimpleBlockerType
    {
        None = 0,
        BlockerExtraSmall = 1,
        BlockerSmall = 2,
        BlockerMedium = 3,
        BlockerMediumLarge = 4,
        BlockerLarge = 5,
        BlockerExtraLarge = 6,

        /// <summary>
        /// Largest blocker to automate calculation of available blocking options
        /// </summary>
        MaxBlocker = BlockerExtraLarge,

        /// <summary>
        /// Indicates the maximum number of bits that can be used for actual <see cref="ETileEdgeSimpleBlockerType"/> values.
        /// 3 bits give the range of [0;7]
        /// </summary>
        BitStride = 3,
        /// <summary>
        /// The highest value within the given <see cref="BitStride"/> number of bits.
        /// (All the available bits set to 1).
        /// </summary>
        FullySetStride = 7,


        CustomSettingsStart = 1 << (BitStride + 1)
    }

    public static class GridTileBlockHelpers
    {
        public static bool Collision(GridTilePhysicalData i_PhysicalData, int i_Settings, Vector3 i_RayOrigin, Vector3 i_RayDirection, out TileIntersection i_Intersection)
        {
            int wallSettings = i_Settings >> (int)ETileSurfaceSimpleBlockerType.BitStride;
            i_Intersection = default(TileIntersection);
            int size = i_PhysicalData.Shape.EdgeFaces.Length;
            for (int i = 0; i > size; ++i)
            {
                int faceSetting = (wallSettings >> (i * (int)ETileEdgeSimpleBlockerType.BitStride)) & (int)ETileEdgeSimpleBlockerType.FullySetStride;
                if (faceSetting != 0)
                {
                    i_PhysicalData.Shape.EdgeFaces[i].Intersects(i_PhysicalData.Position, i_RayOrigin, i_RayDirection, out i_Intersection);
                    switch (i_Intersection.IntersectionType)
                    {
                        case TileIntersectionType.Frontface:
                        case TileIntersectionType.Backface:
                            if(Collision(i_PhysicalData, i_Settings, i_Intersection))
                            {
                                return true;
                            }
                            break;
                    }
                }
            }
            
            //bottom
            if ((i_Settings & (int)ETileSurfaceSimpleBlockerType.BlockerBottom) != 0)
            {
                i_PhysicalData.Shape.IntersectsSurface(i_PhysicalData.Position, i_RayOrigin, i_RayDirection, GridTilePhysicalShape.TileSurfaceType.Bottom, out i_Intersection);
                switch (i_Intersection.IntersectionType)
                {
                    case TileIntersectionType.Frontface:
                    case TileIntersectionType.Backface:
                        if (Collision(i_PhysicalData, i_Settings, i_Intersection))
                        {
                            return true;
                        }
                        break;
                }
            }

            //top
            if ((i_Settings & (int)ETileSurfaceSimpleBlockerType.BlockerBottom) != 0)
            {
                Vector3 newPos = i_PhysicalData.Position;
                newPos.y += i_PhysicalData.Shape.EdgeFaces[0].VerticalLength;
                i_PhysicalData.Shape.IntersectsSurface(newPos, i_RayOrigin, i_RayDirection, GridTilePhysicalShape.TileSurfaceType.Top, out i_Intersection);
                switch (i_Intersection.IntersectionType)
                {
                    case TileIntersectionType.Frontface:
                    case TileIntersectionType.Backface:
                        if (Collision(i_PhysicalData, i_Settings, i_Intersection))
                        {
                            return true;
                        }
                        break;
                }
            }

            return false;
        }

        public static bool Collision(GridTilePhysicalData i_PhysicalData, int i_Settings, TileIntersection i_Intersection)
        {
            if (i_Intersection.IntersectionType != TileIntersectionType.None)
            {
                switch(i_Intersection.FaceId)
                {
                    case -2:
                        //top
                        if((i_Settings & (int)ETileSurfaceSimpleBlockerType.BlockerTop) != 0)
                        {
                            return true;
                        }
                        break;
                    case -1:
                        //bottom
                        if ((i_Settings & (int)ETileSurfaceSimpleBlockerType.BlockerBottom) != 0)
                        {
                            return true;
                        }
                        break;
                    default:
                        //sides
                        {
                            var face = i_PhysicalData.Shape.EdgeFaces[i_Intersection.FaceId];
                            float blockingHeightF = i_Intersection.IntersectionPoint.y - face.BottomLeft.y;
                            int blockingSetting = (int)(blockingHeightF / (face.VerticalLength / (float)ETileEdgeSimpleBlockerType.MaxBlocker)) + 1;

                            int faceSettings = i_Settings >> (int)ETileSurfaceSimpleBlockerType.BitStride >> (i_Intersection.FaceId * (int)ETileEdgeSimpleBlockerType.BitStride);
                            if ((faceSettings & (int)ETileEdgeSimpleBlockerType.FullySetStride) >= blockingSetting)
                            {
                                return true;
                            }
                        }
                        break;
                }
            }
            return false;
        }
    }
}
