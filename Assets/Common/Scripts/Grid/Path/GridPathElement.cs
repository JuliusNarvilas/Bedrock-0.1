using System;
using System.Collections.Generic;

namespace Common.Grid.Path
{
    public enum EGridPathfindingState
    {
        New,
        Opened,
        Closed,

        AvoidanceClosed
    }

    public enum EGridPathAvoidanceStrategy
    {
        DisableAvoidance,
        AvoidAll,
        LowestCombinedAvoidanceLevel,
        LowestAvoidanceCount
    }

    /// <summary>
    /// Class for representing a grid tile pathing data
    /// </summary>
    public class GridPathElement<TPosition, TContext, TTile> : IPoolable where TTile : GridTile<TPosition, TContext, TTile>
    {
        /// <summary>
        /// The tile data that is accessible to users.
        /// </summary>
        public TTile Tile;
        /// <summary>
        /// The approximate distance to the destination (must not be an underestimate).
        /// </summary>
        public int HeuristicDistance;
        /// <summary>
        /// The path cost so far.
        /// </summary>
        public float PathCost;
        /// <summary>
        /// The estimated path cost to the destination.
        /// </summary>
        public float FValue;
        /// <summary>
        /// Current pathing inspection state.
        /// </summary>
        public EGridPathfindingState PathingState = EGridPathfindingState.New;

        /// <summary>
        /// The parent element
        /// </summary>
        public GridPathElement<TPosition, TContext, TTile> Parent;



        /// <summary>
        /// The path cost so far with the avoidance system.
        /// </summary>
        public float AvoidedPathCost;

        public int AvoidanceLevel;
        public int AvoidanceCount;


        public GridPathElement<TPosition, TContext, TTile> AvoidedParent;

        /// <summary>
        /// Initializes a new instance of the <see cref="GridElement"/> class.
        /// </summary>
        public GridPathElement()
        {}

        /// <summary>
        /// Method for cleaning up pathing data back to default state.
        /// </summary>
        public void PoolingClear()
        {
            Tile = default(TTile);
            HeuristicDistance = 0;
            PathCost = 0;
            FValue = 0;
            PathingState = EGridPathfindingState.New;
            Parent = null;
            AvoidedPathCost = 0;
            AvoidanceLevel = 0;
            AvoidanceCount = 0;
            AvoidedParent = null;
        }

        /// <summary>
        /// Copies given instance.
        /// </summary>
        /// <param name="i_Other">The other instance to copy.</param>
        public void Set(GridPathElement<TPosition, TContext, TTile> i_Other)
        {
            Tile = i_Other.Tile;
            HeuristicDistance = i_Other.HeuristicDistance;
            PathCost = i_Other.PathCost;
            FValue = i_Other.FValue;
            PathingState = i_Other.PathingState;
            Parent = i_Other.Parent;
            AvoidedPathCost = i_Other.AvoidedPathCost;
            AvoidanceLevel = i_Other.AvoidanceLevel;
            AvoidanceCount = i_Other.AvoidanceCount;
            AvoidedParent = i_Other.AvoidedParent;
        }

        public class FValueComparer : IComparer<GridPathElement<TPosition, TContext, TTile>>
        {
            private int m_Modifier;

            private FValueComparer(bool i_Ascending)
            {
                m_Modifier = i_Ascending ? 1 : -1;
            }

            public int Compare(GridPathElement<TPosition, TContext, TTile> i_A, GridPathElement<TPosition, TContext, TTile> i_B)
            {
                return i_A.FValue.CompareTo(i_B.FValue) * m_Modifier;
            }

            public static readonly FValueComparer Ascending = new FValueComparer(true);
            public static readonly FValueComparer Descending = new FValueComparer(false);
        }

        public class AvoidanceLevelComparer : IComparer<GridPathElement<TPosition, TContext, TTile>>
        {
            int m_Modifier;

            private AvoidanceLevelComparer(bool i_Ascending)
            {
                m_Modifier = i_Ascending ? 1 : -1;
            }

            public int Compare(GridPathElement<TPosition, TContext, TTile> x, GridPathElement<TPosition, TContext, TTile> y)
            {
                var result = x.AvoidanceLevel.CompareTo(y.AvoidanceLevel) * m_Modifier;
                if(result == 0)
                {
                    result = x.FValue.CompareTo(y.FValue) * m_Modifier;
                }
                return result;
            }

            public static readonly AvoidanceLevelComparer Ascending = new AvoidanceLevelComparer(true);
            public static readonly AvoidanceLevelComparer Descending = new AvoidanceLevelComparer(false);
        }
        public class AvoidanceCountComparer : IComparer<GridPathElement<TPosition, TContext, TTile>>
        {
            int m_Modifier;

            private AvoidanceCountComparer(bool i_Ascending)
            {
                m_Modifier = i_Ascending ? 1 : -1;
            }

            public int Compare(GridPathElement<TPosition, TContext, TTile> x, GridPathElement<TPosition, TContext, TTile> y)
            {
                var result = x.AvoidanceCount.CompareTo(y.AvoidanceCount) * m_Modifier;
                if (result == 0)
                {
                    result = x.FValue.CompareTo(y.FValue) * m_Modifier;
                }
                return result;
            }

            public static readonly AvoidanceCountComparer Ascending = new AvoidanceCountComparer(true);
            public static readonly AvoidanceCountComparer Descending = new AvoidanceCountComparer(false);
        }
    }
}
