using System.Collections.Generic;

namespace Common.Grid.Path
{
    public enum GridPathfindingState
    {
        New,
        Opened,
        Closed
    }

    /// <summary>
    /// Class for reprisenting a grid tile pathing data
    /// </summary>
    public class GridPathElement<TTile, TTerrain, TPosition, TContext>
        where TTile : GridTile<TTerrain, TPosition, TContext>
        where TTerrain : GridTerrain<TContext>
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
        public GridPathfindingState PathingState = GridPathfindingState.New;

        /// <summary>
        /// The parent element
        /// </summary>
        public GridPathElement<TTile, TTerrain, TPosition, TContext> Parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="GridElement"/> class.
        /// </summary>
        public GridPathElement()
        {}

        /// <summary>
        /// Method for cleaning up pathing data back to default state.
        /// </summary>
        public void Clear()
        {
            Tile = null;
            HeuristicDistance = 0;
            PathCost = 0;
            FValue = 0;
            PathingState = GridPathfindingState.New;
            Parent = null;
        }

        /// <summary>
        /// Copies given instance.
        /// </summary>
        /// <param name="i_Other">The other instance to copy.</param>
        public void Set(GridPathElement<TTile, TTerrain, TPosition, TContext> i_Other)
        {
            HeuristicDistance = i_Other.HeuristicDistance;
            PathCost = i_Other.PathCost;
            FValue = i_Other.FValue;
            PathingState = i_Other.PathingState;
            Parent = i_Other.Parent;
        }

        public class FValueComparer : IComparer<GridPathElement<TTile, TTerrain, TPosition, TContext>>
        {
            private int m_Modifier;

            private FValueComparer(bool i_Ascending)
            {
                m_Modifier = i_Ascending ? 1 : -1;
            }

            public int Compare(GridPathElement<TTile, TTerrain, TPosition, TContext> i_A, GridPathElement<TTile, TTerrain, TPosition, TContext> i_B)
            {
                return i_A.FValue.CompareTo(i_B.FValue) * m_Modifier;
            }

            public static FValueComparer Ascending = new FValueComparer(true);
            public static FValueComparer Descending = new FValueComparer(false);
        }
    }
}
