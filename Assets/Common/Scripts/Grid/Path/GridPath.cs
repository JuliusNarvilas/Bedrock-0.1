using Common.Collections;
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
    public class GridPath<TPosition, TTileData, TContext> : IDisposable
    {
        private readonly IGridControl<TPosition, TTileData, TContext> m_Grid;
        private IGridPathData<TPosition, TTileData, TContext> m_GridPathData;

        private List<GridPathElement<TPosition, TTileData, TContext>> m_OpenList = new List<GridPathElement<TPosition, TTileData, TContext>>();
        private List<GridPathElement<TPosition, TTileData, TContext>> m_ClosedList = new List<GridPathElement<TPosition, TTileData, TContext>>();
        private TContext m_Context;

        private List<GridTile<TPosition, TTileData, TContext>> m_ConnectedList = new List<GridTile<TPosition, TTileData, TContext>>();
        private GridPathElement<TPosition, TTileData, TContext> m_FinishElement;

        /// <summary>
        /// Gets the path grid tiles from starting to finishing locations.
        /// </summary>
        /// <remarks>
        /// Returns an empty list on unreachable paths.
        /// </remarks>
        /// <value>
        /// Path grid tiles.
        /// </value>
        public List<GridPathElement<TPosition, TTileData, TContext>> Tiles
        {
            get { return m_OpenList; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Grid2DPath" /> class, creating a tile sequence for quickest path between given positions.
        /// </summary>
        /// <param name="i_Grid">The grid.</param>
        /// <param name="i_PathData">The path data.</param>
        /// <param name="i_StartPos">The start position.</param>
        /// <param name="i_EndPosition">The end position.</param>
        /// <param name="i_Context">The context.</param>
        public GridPath(
            IGridControl<TPosition, TTileData, TContext> i_Grid,
            IGridPathData<TPosition, TTileData, TContext> i_PathData,
            TPosition i_StartPos, TPosition i_EndPosition,
            TContext i_Context
        )
        {
            m_Grid = i_Grid;
            m_GridPathData = i_PathData;
            m_Context = i_Context;
            m_FinishElement = GetElementOrDefault(i_EndPosition);
            
            //if finish exists
            if (m_FinishElement != null)
            {
                var startElement = GetElementOrDefault(i_StartPos);
                var currentElement = startElement;
                //while a path option exists and finish not reached
                while ((currentElement != null) && (currentElement != m_FinishElement))
                {
                    Close(currentElement);
                    OpenNeighbours(currentElement);
                    currentElement = PickNext();
                }
                //Close the finishing tile if it was opened (it was reached) or it is also the starting point
                if ((startElement == m_FinishElement) || (m_FinishElement.PathingState == GridPathfindingState.Opened))
                {
                    Close(m_FinishElement);
                }
            }

            Finish();
        }

        private GridPathElement<TPosition, TTileData, TContext> GetElementOrDefault(TPosition i_Pos)
        {
            GridPathElement<TPosition, TTileData, TContext> result;
            if (m_GridPathData.TryGetElement(i_Pos, out result) == GridPathDataResponse.OutOfDataRange)
            {
                    m_GridPathData.Grow(i_Pos);
                    m_GridPathData.TryGetElement(i_Pos, out result);
            }
            return result;
        }

        private void Open(GridPathElement<TPosition, TTileData, TContext> i_Element, GridPathElement<TPosition, TTileData, TContext> i_Parent)
        {
            // move terrain cost
            float terrainCost = i_Element.Tile.GetCost(m_Grid, i_Parent, m_Context);
            if (terrainCost >= 0.0f)
            {
                i_Element.HeuristicDistance = m_Grid.GetHeuristicDistance(i_Element.Tile.Position, m_FinishElement.Tile.Position);
                i_Element.PathCost = terrainCost + i_Parent.PathCost; //cost of the path so far
                i_Element.FValue = i_Element.PathCost + i_Element.HeuristicDistance + i_Element.Tile.GetDangerFactor();

                i_Element.PathingState = GridPathfindingState.Opened;
                i_Element.Parent = i_Parent;
                m_OpenList.Add(i_Element);
            }
        }

        private bool Reopen(GridPathElement<TPosition, TTileData, TContext> i_Element, GridPathElement<TPosition, TTileData, TContext> i_Parent)
        {
            float terrainCost = i_Element.Tile.GetCost(m_Grid, i_Parent, m_Context);
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

        private void Close(GridPathElement<TPosition, TTileData, TContext> i_Element)
        {
            i_Element.PathingState = GridPathfindingState.Closed;
            m_ClosedList.Add(i_Element);
        }

        private void OpenNeighbours(GridPathElement<TPosition, TTileData, TContext> i_Element)
        {
            m_Grid.GetConnected(i_Element.Tile.Position, m_ConnectedList);

            int openListSizeBefore = m_OpenList.Count;
            bool oldElementChanged = false;

            int size = m_ConnectedList.Count;
            GridPathElement<TPosition, TTileData, TContext> neighbourElement;
            for (var i = 0; i < size; ++i)
            {
                neighbourElement = GetElementOrDefault(m_ConnectedList[i].Position);
                if (neighbourElement != null)
                {
                    switch (neighbourElement.PathingState)
                    {
                        case GridPathfindingState.New:
                            Open(neighbourElement, i_Element);
                            break;
                        case GridPathfindingState.Opened:
                            oldElementChanged = Reopen(neighbourElement, i_Element) || oldElementChanged;
                            break;
                    }
                }
            }
            if (oldElementChanged || (openListSizeBefore > m_OpenList.Count))
            {
                m_OpenList.InsertionSort(GridPathElement<TPosition, TTileData, TContext>.FValueComparer.Descending);
            }
            m_ConnectedList.Clear();
        }

        private void Finish()
        {
            //clean unused elements
            int openListElementCount = m_OpenList.Count;
            m_OpenList.Clear();

            int maxPathTileCount = m_ClosedList.Count;
            //if finish exists and was reached
            if ((m_FinishElement != null) && (m_FinishElement.PathingState == GridPathfindingState.Closed))
            {
                if(m_OpenList.Capacity < maxPathTileCount)
                {
                    m_OpenList.Capacity = maxPathTileCount;
                }
                FillPathResult(m_FinishElement, 0);
            }
            else
            {
                m_OpenList = null;
            }
            m_ClosedList = null;

            m_FinishElement = null;
            m_GridPathData.Dispose();
            m_GridPathData = null;
        }

        private GridPathElement<TPosition, TTileData, TContext> FillPathResult(GridPathElement<TPosition, TTileData, TContext> i_Element, int counter)
        {
            if(i_Element != null)
            {
                var inserted = FillPathResult(i_Element.Parent, ++counter);
                var resultElement = m_OpenList[m_OpenList.Count - counter];
                resultElement.Set(i_Element);
                resultElement.Parent = inserted;
                
                return resultElement;
            }
            else
            {
                GridPathElementPool<TPosition, TTileData, TContext>.GLOBAL.GetMultiple(counter, m_OpenList);
            }
            return null;
        }

        private GridPathElement<TPosition, TTileData, TContext> PickNext()
        {
            int lastIndex = m_OpenList.Count - 1;
            GridPathElement<TPosition, TTileData, TContext> pick = null;
            if (lastIndex >= 0)
            {
                pick = m_OpenList[lastIndex];
                m_OpenList.RemoveAt(lastIndex);
            }

            return pick;
        }

        public void Dispose()
        {
            if (m_OpenList != null)
            {
                GridPathElementPool<TPosition, TTileData, TContext>.GLOBAL.RecycleMultiple(m_OpenList);
                m_OpenList = null;
            }
        }
    }
}
