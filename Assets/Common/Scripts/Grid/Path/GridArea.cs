using System;
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
    public class GridArea<TPosition, TTileData, TContext> : IDisposable
    {
        public readonly IGridControl<TPosition, TTileData, TContext> Grid;
        public readonly IGridPathData<TPosition, TTileData, TContext> GridPathData;
        public readonly TPosition Min;
        public readonly TPosition Max;
        public readonly TPosition Origin;
        private TContext m_Context;

        private readonly Queue<GridPathElement<TPosition, TTileData, TContext>> m_OpenQueue = new Queue<GridPathElement<TPosition, TTileData, TContext>>();
        private readonly List<GridTile<TPosition, TTileData, TContext>> m_ConnectedList = new List<GridTile<TPosition, TTileData, TContext>>();

        public GridArea(
            IGridControl<TPosition, TTileData, TContext> i_Grid,
            IGridPathData<TPosition, TTileData, TContext> i_PathData,
            TPosition i_Min, TPosition i_Max, TPosition i_Origin,
            TContext i_Context
        )
        {
            Grid = i_Grid;
            GridPathData = i_PathData;
            Min = i_Min;
            Max = i_Max;
            Origin = i_Origin;
            m_Context = i_Context;
            
            GridPathElement<TPosition, TTileData, TContext> originElement;
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

        private void Open(GridPathElement<TPosition, TTileData, TContext> i_Element, GridPathElement<TPosition, TTileData, TContext> i_Parent)
        {
            // move terrain cost
            float terrainCost = i_Element.Tile.GetCost(Grid, i_Parent, m_Context);
            if (terrainCost >= 0.0f)
            {
                i_Element.PathCost = terrainCost + i_Parent.PathCost; //cost of the path so far
                i_Element.FValue = i_Element.PathCost + i_Element.Tile.GetDangerFactor();

                i_Element.PathingState = GridPathfindingState.Opened;
                i_Element.Parent = i_Parent;
                m_OpenQueue.Enqueue(i_Element);
            }
        }

        private bool Reopen(GridPathElement<TPosition, TTileData, TContext> i_Element, GridPathElement<TPosition, TTileData, TContext> i_Parent)
        {
            float terrainCost = i_Element.Tile.GetCost(Grid, i_Parent, m_Context);
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

        private void OpenNeighbours(GridPathElement<TPosition, TTileData, TContext> i_Element)
        {
            Grid.GetConnected(i_Element.Tile.Position, m_ConnectedList);
            int size = m_ConnectedList.Count;
            for (var i = 0; i < size; ++i)
            {
                GridPathElement<TPosition, TTileData, TContext> neighbourElement;
                //only continue if position is within GridPathData area 
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

        public void Dispose()
        {
            GridPathData.Dispose();
        }
    }
}
