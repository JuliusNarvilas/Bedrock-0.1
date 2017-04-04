
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Common.Grid
{
    public enum GridTileBlockerHeight : int
    {
        None = 0,
        SmallBlocker = 1,
        MediumBlocker = 2,
        LargeBlocker = 3,
        ExtraLargeBlocker = 4,

        BitStride = 3,
        FullySetStride = 7
    }
    public enum GridTileLocation : int
    {
        Left = 0,
        Right = 1,
        Forward = 2,
        Backward = 3,
        Bottom = 4,
        Top = 5
    }

    [Flags]
    public enum GridTileBlockerFlags : int
    {
        None = 0,

        LeftSmallBlocker = GridTileBlockerHeight.SmallBlocker << (GridTileBlockerHeight.BitStride * GridTileLocation.Left),
        LeftMediumBlocker = GridTileBlockerHeight.MediumBlocker << (GridTileBlockerHeight.BitStride * GridTileLocation.Left),
        LeftLargeBlocker = GridTileBlockerHeight.LargeBlocker << (GridTileBlockerHeight.BitStride * GridTileLocation.Left),
        LeftExtraLargeBlocker = GridTileBlockerHeight.ExtraLargeBlocker << (GridTileBlockerHeight.BitStride * GridTileLocation.Left),
        LeftAnyBlocker = GridTileBlockerHeight.FullySetStride << (GridTileBlockerHeight.BitStride * GridTileLocation.Left),

        RightSmallBlocker = GridTileBlockerHeight.SmallBlocker << (GridTileBlockerHeight.BitStride * GridTileLocation.Right),
        RightMediumBlocker = GridTileBlockerHeight.MediumBlocker << (GridTileBlockerHeight.BitStride * GridTileLocation.Right),
        RightLargeBlocker = GridTileBlockerHeight.LargeBlocker << (GridTileBlockerHeight.BitStride * GridTileLocation.Right),
        RightExtraLargeBlocker = GridTileBlockerHeight.ExtraLargeBlocker << (GridTileBlockerHeight.BitStride * GridTileLocation.Right),
        RightAnyBlocker = GridTileBlockerHeight.FullySetStride << (GridTileBlockerHeight.BitStride * GridTileLocation.Right),

        ForwardSmallBlocker = GridTileBlockerHeight.SmallBlocker << (GridTileBlockerHeight.BitStride * GridTileLocation.Forward),
        ForwardMediumBlocker = GridTileBlockerHeight.MediumBlocker << (GridTileBlockerHeight.BitStride * GridTileLocation.Forward),
        ForwardLargeBlocker = GridTileBlockerHeight.LargeBlocker << (GridTileBlockerHeight.BitStride * GridTileLocation.Forward),
        ForwardExtraLargeBlocker = GridTileBlockerHeight.ExtraLargeBlocker << (GridTileBlockerHeight.BitStride * GridTileLocation.Forward),
        ForwardAnyBlocker = GridTileBlockerHeight.FullySetStride << (GridTileBlockerHeight.BitStride * GridTileLocation.Forward),

        BackwardSmallBlocker = GridTileBlockerHeight.SmallBlocker << (GridTileBlockerHeight.BitStride * GridTileLocation.Backward),
        BackwardMediumBlocker = GridTileBlockerHeight.MediumBlocker << (GridTileBlockerHeight.BitStride * GridTileLocation.Backward),
        BackwardLargeBlocker = GridTileBlockerHeight.LargeBlocker << (GridTileBlockerHeight.BitStride * GridTileLocation.Backward),
        BackwardExtraLargeBlocker = GridTileBlockerHeight.ExtraLargeBlocker << (GridTileBlockerHeight.BitStride * GridTileLocation.Backward),
        BackwardAnyBlocker = GridTileBlockerHeight.FullySetStride << (GridTileBlockerHeight.BitStride * GridTileLocation.Backward),

        BottomBlocker = 1 << (GridTileBlockerHeight.BitStride * GridTileLocation.Bottom),
        TopBlocker = BottomBlocker << 1,

        CustomFlagStart = TopBlocker << 1
    }

    public static class GridHelpers
    {
        public const int GRID_TILE_LOCATION_STRIDE_MASK = 7;

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
