using System.Collections.Generic;

namespace Common.Grid.Path
{
    /// <summary>
    /// Grid pathfinding functionality.
    /// </summary>
    /// <typeparam name="TTile">The type of the tile.</typeparam>
    /// <typeparam name="TTerrain">The type of the terrain.</typeparam>
    /// <typeparam name="TPosition">The type of the position.</typeparam>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    /// <seealso cref="System.IDisposable" />
    public class GridArea<TTile, TTerrain, TPosition, TContext>
        where TTile : GridTile<TTerrain, TPosition, TContext>
        where TTerrain : GridTerrain<TContext>
    {
        public readonly IGridControl<TTile, TTerrain, TPosition, TContext> Grid;
        public readonly IGridPathData<TTile, TTerrain, TPosition, TContext> GridPathData;
        public readonly TPosition Min;
        public readonly TPosition Max;
        public readonly TPosition Origin;
        public TContext Context;

        private readonly Queue<GridPathElement<TTile, TTerrain, TPosition, TContext>> m_OpenQueue = new Queue<GridPathElement<TTile, TTerrain, TPosition, TContext>>();
        private readonly List<TTile> m_ConnectedList = new List<TTile>();

        public GridArea(
            IGridControl<TTile, TTerrain, TPosition, TContext> i_Grid,
            IGridPathData<TTile, TTerrain, TPosition, TContext> i_PathData,
            TPosition i_Min, TPosition i_Max, TPosition i_Origin,
            TContext i_Context
        )
        {
            Grid = i_Grid;
            GridPathData = i_PathData;
            GridPathData.Set(i_Grid, i_Min, i_Max);
            Min = i_Min;
            Max = i_Max;
            Origin = i_Origin;
            Context = i_Context;
            
            GridPathElement<TTile, TTerrain, TPosition, TContext> originElement;
            if (i_PathData.TryGetElement(i_Origin, out originElement) == GridPathDataResponse.Success)
            {
                OpenNeighbours(originElement);
                while (m_OpenQueue.Count > 0)
                {
                    OpenNeighbours(m_OpenQueue.Dequeue());
                }
            }
            m_OpenQueue = null;
            m_ConnectedList = null;
        }

        private void Open(GridPathElement<TTile, TTerrain, TPosition, TContext> i_Element, GridPathElement<TTile, TTerrain, TPosition, TContext> i_Parent)
        {
            // move terrain cost
            float terrainCost = i_Parent.Tile.GetTransitionOutCost(i_Element.Tile, Context);
            terrainCost += i_Element.Tile.GetTransitionInCost(i_Parent.Tile, Context);
            if (terrainCost >= 0.0f)
            {
                i_Element.PathCost = terrainCost + i_Parent.PathCost; //cost of the path so far
                i_Element.FValue = i_Element.PathCost + i_Element.Tile.GetDangerFactor();

                i_Element.PathingState = GridPathfindingState.Opened;
                i_Element.Parent = i_Parent;
                m_OpenQueue.Enqueue(i_Element);
            }
        }

        private bool Reopen(GridPathElement<TTile, TTerrain, TPosition, TContext> i_Element, GridPathElement<TTile, TTerrain, TPosition, TContext> i_Parent)
        {
            float terrainCost = i_Parent.Tile.GetTransitionOutCost(i_Element.Tile, Context);
            terrainCost += i_Element.Tile.GetTransitionInCost(i_Parent.Tile, Context);
            //negative cost indicates blockers
            if (terrainCost >= 0)
            {
                float newPathCost = (terrainCost + i_Parent.PathCost);
                if (newPathCost < i_Element.PathCost)
                {
                    i_Element.PathCost = newPathCost;
                    i_Element.FValue = newPathCost + i_Element.HeuristicDistance + i_Element.Tile.GetDangerFactor();
                    i_Element.Parent = i_Parent;
                    return true;
                }
            }
            return false;
        }

        private void OpenNeighbours(GridPathElement<TTile, TTerrain, TPosition, TContext> i_Element)
        {
            Grid.GetConnected(i_Element.Tile.Position, m_ConnectedList);
            int size = m_ConnectedList.Count;
            for (var i = 0; i < size; ++i)
            {
                GridPathElement<TTile, TTerrain, TPosition, TContext> neighbourElement;
                if (GridPathData.TryGetElement(m_ConnectedList[i].Position, out neighbourElement) == GridPathDataResponse.Success)
                {
                    switch (neighbourElement.PathingState)
                    {
                        case GridPathfindingState.New:
                            Open(neighbourElement, i_Element);
                            break;
                        case GridPathfindingState.Opened:
                            Reopen(neighbourElement, i_Element);
                            break;
                    }
                }
            }
            m_ConnectedList.Clear();
        }
    }
}
