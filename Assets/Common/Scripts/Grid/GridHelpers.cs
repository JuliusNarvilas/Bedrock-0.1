
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Common.Grid
{
    public enum GridTileBlockerHeights : int
    {
        None = 0,
        SmallBlocker = 1,
        MediumBlocker = 2,
        LargeBlocker = 3,
        ExtraLargeBlocker = 4,

        BitStride = 3,
        FullySetStride = 7
    }
    public enum GridTileBlockerLocations : int
    {
        LeftBlocker = 0,
        RightBlocker = 1,
        ForwardBlocker = 2,
        BackwardBlocker = 3,
        BottomBlocker = 4,
        TopBlocker = 5
    }

    [Flags]
    public enum GridTileBlockerFlags : int
    {
        None = 0,

        LeftSmallBlocker = GridTileBlockerHeights.SmallBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.LeftBlocker),
        LeftMediumBlocker = GridTileBlockerHeights.MediumBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.LeftBlocker),
        LeftLargeBlocker = GridTileBlockerHeights.LargeBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.LeftBlocker),
        LeftExtraLargeBlocker = GridTileBlockerHeights.ExtraLargeBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.LeftBlocker),
        LeftAnyBlocker = GridTileBlockerHeights.FullySetStride << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.LeftBlocker),

        RightSmallBlocker = GridTileBlockerHeights.SmallBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.RightBlocker),
        RightMediumBlocker = GridTileBlockerHeights.MediumBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.RightBlocker),
        RightLargeBlocker = GridTileBlockerHeights.LargeBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.RightBlocker),
        RightExtraLargeBlocker = GridTileBlockerHeights.ExtraLargeBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.RightBlocker),
        RightAnyBlocker = GridTileBlockerHeights.FullySetStride << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.RightBlocker),

        ForwardSmallBlocker = GridTileBlockerHeights.SmallBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.ForwardBlocker),
        ForwardMediumBlocker = GridTileBlockerHeights.MediumBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.ForwardBlocker),
        ForwardLargeBlocker = GridTileBlockerHeights.LargeBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.ForwardBlocker),
        ForwardExtraLargeBlocker = GridTileBlockerHeights.ExtraLargeBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.ForwardBlocker),
        ForwardAnyBlocker = GridTileBlockerHeights.FullySetStride << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.ForwardBlocker),

        BackwardSmallBlocker = GridTileBlockerHeights.SmallBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.BackwardBlocker),
        BackwardMediumBlocker = GridTileBlockerHeights.MediumBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.BackwardBlocker),
        BackwardLargeBlocker = GridTileBlockerHeights.LargeBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.BackwardBlocker),
        BackwardExtraLargeBlocker = GridTileBlockerHeights.ExtraLargeBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.BackwardBlocker),
        BackwardAnyBlocker = GridTileBlockerHeights.FullySetStride << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.BackwardBlocker),

        BottomBlocker = 1 << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.BottomBlocker),
        TopBlocker = BottomBlocker << 1,

        CustomFlagStart = TopBlocker << 1
    }

    public static class GridHelpers
    {
        public static readonly int[] BLOCKER_ENUM_VALUE_MAP = new int[] {
            (int)GridTileBlockerFlags.None,
            (int)GridTileBlockerFlags.LeftSmallBlocker,
            (int)GridTileBlockerFlags.LeftMediumBlocker,
            (int)GridTileBlockerFlags.LeftLargeBlocker,
            (int)GridTileBlockerFlags.LeftExtraLargeBlocker,
            (int)GridTileBlockerFlags.LeftAnyBlocker,

            (int)GridTileBlockerFlags.RightSmallBlocker,
            (int)GridTileBlockerFlags.RightMediumBlocker,
            (int)GridTileBlockerFlags.RightLargeBlocker,
            (int)GridTileBlockerFlags.RightExtraLargeBlocker,
            (int)GridTileBlockerFlags.RightAnyBlocker,

            (int)GridTileBlockerFlags.ForwardSmallBlocker,
            (int)GridTileBlockerFlags.ForwardMediumBlocker,
            (int)GridTileBlockerFlags.ForwardLargeBlocker,
            (int)GridTileBlockerFlags.ForwardExtraLargeBlocker,
            (int)GridTileBlockerFlags.ForwardAnyBlocker,

            (int)GridTileBlockerFlags.BackwardSmallBlocker,
            (int)GridTileBlockerFlags.BackwardMediumBlocker,
            (int)GridTileBlockerFlags.BackwardLargeBlocker,
            (int)GridTileBlockerFlags.BackwardExtraLargeBlocker,
            (int)GridTileBlockerFlags.BackwardAnyBlocker,

            (int)GridTileBlockerFlags.BottomBlocker,
            (int)GridTileBlockerFlags.TopBlocker,

            (int)GridTileBlockerFlags.CustomFlagStart
        };

        //Converter for unity's shity mask support
        public static int BlockerIndexFlagsToValue(int i_IndexFlags)
        {
            int result = 0;
            for (int i = 0; i < BLOCKER_ENUM_VALUE_MAP.Length; ++i)
            {
                if ((i_IndexFlags & (1 << i)) != 0)
                {
                    result |= BLOCKER_ENUM_VALUE_MAP[i];
                }
            }
            return result;
        }

        public static int BlockerValueToIndexFlags(int i_Value)
        {
            int result = 0;
            for(int i = 0; i < BLOCKER_ENUM_VALUE_MAP.Length; ++i)
            {
                if((i_Value & BLOCKER_ENUM_VALUE_MAP[i]) == BLOCKER_ENUM_VALUE_MAP[i])
                {
                    if (BLOCKER_ENUM_VALUE_MAP[i] == 0 && i_Value != 0)
                        continue;
                    result |= 1 << i;
                }
            }
            return result;
        }
    }

    public interface IGridObjectReference<TPosition, TTileData, TContext>
    {
        GridObject<TPosition, TTileData, TContext> GetGridobject();
    }

    public abstract class GridObject<TPosition, TTileData, TContext>
    {
        public Vector3 Position;
        public Vector3 Scale;
        public Quaternion Rotation;

        public TPosition Origin;
        public int Settings;
        public List<PairStruct<TPosition, TTileData>> DataList;

        public List<GridObject<TPosition, TTileData, TContext>> Children;

        public abstract void Damage(IGridControl<TPosition, TTileData, TContext> i_Grid, TPosition i_Position, TContext i_Context);
    }
}
