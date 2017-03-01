
using System;
using System.Collections.Generic;

namespace Common.Grid
{
    public static class GridHelpers
    {
        private enum GridTileBlockerHeights : int
        {
            None = 0,
            SmallBlocker = 1,
            MediumBlocker = 2,
            LargeBlocker = 3,
            ExtraLargeBlocker = 4,

            BitStride = 3,
            FullySetStride = 7
        }
        private enum GridTileBlockerLocations : int
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

            LeftSmallBlocker        = GridTileBlockerHeights.SmallBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.LeftBlocker),
            LeftMediumBlocker       = GridTileBlockerHeights.MediumBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.LeftBlocker),
            LeftLargeBlocker        = GridTileBlockerHeights.LargeBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.LeftBlocker),
            LeftExtraLargeBlocker   = GridTileBlockerHeights.ExtraLargeBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.LeftBlocker),
            LeftAnyBlocker          = GridTileBlockerHeights.FullySetStride << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.LeftBlocker),

            RightSmallBlocker       = GridTileBlockerHeights.SmallBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.RightBlocker),
            RightMediumBlocker      = GridTileBlockerHeights.MediumBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.RightBlocker),
            RightLargeBlocker       = GridTileBlockerHeights.LargeBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.RightBlocker),
            RightExtraLargeBlocker  = GridTileBlockerHeights.ExtraLargeBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.RightBlocker),
            RightAnyBlocker         = GridTileBlockerHeights.FullySetStride << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.RightBlocker),

            ForwardSmallBlocker         = GridTileBlockerHeights.SmallBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.ForwardBlocker),
            ForwardMediumBlocker        = GridTileBlockerHeights.MediumBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.ForwardBlocker),
            ForwardLargeBlocker         = GridTileBlockerHeights.LargeBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.ForwardBlocker),
            ForwardExtraLargeBlocker    = GridTileBlockerHeights.ExtraLargeBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.ForwardBlocker),
            ForwardAnyBlocker           = GridTileBlockerHeights.FullySetStride << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.ForwardBlocker),

            BackwardSmallBlocker        = GridTileBlockerHeights.SmallBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.BackwardBlocker),
            BackwardMediumBlocker       = GridTileBlockerHeights.MediumBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.BackwardBlocker),
            BackwardLargeBlocker        = GridTileBlockerHeights.LargeBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.BackwardBlocker),
            BackwardExtraLargeBlocker   = GridTileBlockerHeights.ExtraLargeBlocker << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.BackwardBlocker),
            BackwardAnyBlocker          = GridTileBlockerHeights.FullySetStride << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.BackwardBlocker),

            BottomBlocker = 1 << (GridTileBlockerHeights.BitStride * GridTileBlockerLocations.BottomBlocker),
            TopBlocker = BottomBlocker << 1,

            CustomFlagStart = TopBlocker << 1
        }
    }

    public abstract class GridObject<TPosition, TTileData, TContext>
    {
        public TPosition Origin;
        public int Settings;
        public List<PairStruct<TPosition, TTileData>> DataList;

        public List<GridObject<TPosition, TTileData, TContext>> Children;

        public abstract void Damage(IGridControl<TPosition, TTileData, TContext> i_Grid, TPosition i_Position, TContext i_Context);
    }
}
