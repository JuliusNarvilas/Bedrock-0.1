using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Common.Grid.Physics
{
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
        public static void Collision(GridTilePhysicalData i_PhysicalData, int i_Settings, Vector3 i_RayOrigin, Vector3 i_RayDirection)
        {
            if((i_Settings & (int)ETileSurfaceSimpleBlockerType.BlockerTop) != 0)
            {
                //TODO
            }

            if ((i_Settings & (int)ETileSurfaceSimpleBlockerType.BlockerBottom) != 0)
            {
                //TODO
            }

            i_Settings = i_Settings >> (int)ETileSurfaceSimpleBlockerType.BitStride;

            int size = i_PhysicalData.Shape.EdgeFaces.Length;
            for(int i = 0; i < size; ++i)
            {
                int blockerSetting = i_Settings & (int)ETileEdgeSimpleBlockerType.FullySetStride;
                if (blockerSetting != 0)
                {
                    //TODO
                }
                i_Settings = i_Settings >> (int)ETileEdgeSimpleBlockerType.BitStride;
            }
        }

        public static bool Collision(GridTilePhysicalData i_PhysicalData, int i_Settings, TileIntersection i_Intersection)
        {
            if(i_Intersection.IntersectionType == TileIntersectionType.
            if ((i_Settings & (int)ETileSurfaceSimpleBlockerType.BlockerTop) != 0)
            {

            }

            if ((i_Settings & (int)ETileSurfaceSimpleBlockerType.BlockerBottom) != 0)
            {
                //TODO
            }

            i_Settings = i_Settings >> (int)ETileSurfaceSimpleBlockerType.BitStride;

            int size = i_PhysicalData.Shape.EdgeFaces.Length;
            for (int i = 0; i < size; ++i)
            {
                int blockerSetting = i_Settings & (int)ETileEdgeSimpleBlockerType.FullySetStride;
                if (blockerSetting != 0)
                {
                    //TODO
                }
                i_Settings = i_Settings >> (int)ETileEdgeSimpleBlockerType.BitStride;
            }
        }
    }
}
