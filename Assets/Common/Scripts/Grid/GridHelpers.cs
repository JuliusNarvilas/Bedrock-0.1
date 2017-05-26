
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Common.Grid
{
    /// <summary>
    /// The tile blocker height is a unique numerical identifier with <see cref="BitStride"/> indicating the maximum number of bits used to express it
    /// </summary>
    public enum GridTileBlockerHeight : int
    {
        None = 0,
        SmallBlocker = 1,
        MediumBlocker = 2,
        LargeBlocker = 3,
        ExtraLargeBlocker = 4,

        /// <summary>
        /// Indicates the maximum number of bits that can be used for actual <see cref="GridTileBlockerHeight"/> values.
        /// 3 bits give the range of [0;7]
        /// </summary>
        BitStride = 3,
        /// <summary>
        /// The highest value within the given <see cref="BitStride"/> number of bits.
        /// (All the available bits set to 1).
        /// </summary>
        FullySetStride = 7
    }
    public enum GridTileLocation : int
    {
        Left = 0,
        Forward = 1,
        Right = 2,
        Backward = 3,
        Bottom = 4,
        Top = 5
    }

    /// <summary>
    /// Unique enum shorthand values for blocking heights within small binary flag slots that are indexed for <see cref="GridTileLocation"/> values.
    /// Max binary slot per <see cref="GridTileLocation"/> is defined by <see cref="GridTileBlockerHeight.BitStride"/>
    /// </summary>
    [Flags]
    public enum GridTileBlockerFlags : int
    {
        None = 0,

        LeftSmallBlocker = GridTileBlockerHeight.SmallBlocker << (GridTileBlockerHeight.BitStride * GridTileLocation.Left),
        LeftMediumBlocker = GridTileBlockerHeight.MediumBlocker << (GridTileBlockerHeight.BitStride * GridTileLocation.Left),
        LeftLargeBlocker = GridTileBlockerHeight.LargeBlocker << (GridTileBlockerHeight.BitStride * GridTileLocation.Left),
        LeftExtraLargeBlocker = GridTileBlockerHeight.ExtraLargeBlocker << (GridTileBlockerHeight.BitStride * GridTileLocation.Left),
        LeftAnyBlocker = GridTileBlockerHeight.FullySetStride << (GridTileBlockerHeight.BitStride * GridTileLocation.Left),

        ForwardSmallBlocker = GridTileBlockerHeight.SmallBlocker << (GridTileBlockerHeight.BitStride * GridTileLocation.Forward),
        ForwardMediumBlocker = GridTileBlockerHeight.MediumBlocker << (GridTileBlockerHeight.BitStride * GridTileLocation.Forward),
        ForwardLargeBlocker = GridTileBlockerHeight.LargeBlocker << (GridTileBlockerHeight.BitStride * GridTileLocation.Forward),
        ForwardExtraLargeBlocker = GridTileBlockerHeight.ExtraLargeBlocker << (GridTileBlockerHeight.BitStride * GridTileLocation.Forward),
        ForwardAnyBlocker = GridTileBlockerHeight.FullySetStride << (GridTileBlockerHeight.BitStride * GridTileLocation.Forward),

        RightSmallBlocker = GridTileBlockerHeight.SmallBlocker << (GridTileBlockerHeight.BitStride * GridTileLocation.Right),
        RightMediumBlocker = GridTileBlockerHeight.MediumBlocker << (GridTileBlockerHeight.BitStride * GridTileLocation.Right),
        RightLargeBlocker = GridTileBlockerHeight.LargeBlocker << (GridTileBlockerHeight.BitStride * GridTileLocation.Right),
        RightExtraLargeBlocker = GridTileBlockerHeight.ExtraLargeBlocker << (GridTileBlockerHeight.BitStride * GridTileLocation.Right),
        RightAnyBlocker = GridTileBlockerHeight.FullySetStride << (GridTileBlockerHeight.BitStride * GridTileLocation.Right),

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

        /// <summary>
        /// A map for associating <see cref="GridTileBlockerFlags"/> enum index to the enum value.
        /// Used for conversion between real data and Unity's inspector simple enum interface options.
        /// </summary>
        public static readonly int[] BLOCKER_ENUM_VALUE_MAP = new int[] {
            (int)GridTileBlockerFlags.None,
            (int)GridTileBlockerFlags.LeftSmallBlocker,
            (int)GridTileBlockerFlags.LeftMediumBlocker,
            (int)GridTileBlockerFlags.LeftLargeBlocker,
            (int)GridTileBlockerFlags.LeftExtraLargeBlocker,
            (int)GridTileBlockerFlags.LeftAnyBlocker,

            (int)GridTileBlockerFlags.ForwardSmallBlocker,
            (int)GridTileBlockerFlags.ForwardMediumBlocker,
            (int)GridTileBlockerFlags.ForwardLargeBlocker,
            (int)GridTileBlockerFlags.ForwardExtraLargeBlocker,
            (int)GridTileBlockerFlags.ForwardAnyBlocker,

            (int)GridTileBlockerFlags.RightSmallBlocker,
            (int)GridTileBlockerFlags.RightMediumBlocker,
            (int)GridTileBlockerFlags.RightLargeBlocker,
            (int)GridTileBlockerFlags.RightExtraLargeBlocker,
            (int)GridTileBlockerFlags.RightAnyBlocker,

            (int)GridTileBlockerFlags.BackwardSmallBlocker,
            (int)GridTileBlockerFlags.BackwardMediumBlocker,
            (int)GridTileBlockerFlags.BackwardLargeBlocker,
            (int)GridTileBlockerFlags.BackwardExtraLargeBlocker,
            (int)GridTileBlockerFlags.BackwardAnyBlocker,

            (int)GridTileBlockerFlags.BottomBlocker,
            (int)GridTileBlockerFlags.TopBlocker,

            (int)GridTileBlockerFlags.CustomFlagStart
        };

        /// <summary>
        /// Conversion from Unity's inspector flag options to real compressed data.
        /// </summary>
        /// <param name="i_IndexFlags">Flag mask indicating <see cref="GridTileBlockerFlags"/> enum index (not actual value) selections.</param>
        /// <returns>Real compressed blocker data.</returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i_Value"></param>
        /// <returns></returns>
        public static int BlockerValueToIndexFlags(int i_Value)
        {
            bool startedAsZero = i_Value == 0;
            int result = 0;
            for (int i = BLOCKER_ENUM_VALUE_MAP.Length - 1; i >= 0; --i)
            {
                int enumBlockerValue = BLOCKER_ENUM_VALUE_MAP[i];
                if ((i_Value & enumBlockerValue) == enumBlockerValue)
                {
                    if (enumBlockerValue == 0 && startedAsZero)
                        continue;
                    result |= 1 << i;
                    i_Value ^= enumBlockerValue;
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
