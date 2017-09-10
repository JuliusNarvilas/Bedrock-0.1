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
    public class GridPath<TPosition, TContext, TTile> : IDisposable where TTile : GridTile<TPosition, TContext, TTile>
    {
        private readonly IGridControl<TPosition, TContext, TTile> m_Grid;
        private IGridPathData<TPosition, TContext, TTile> m_GridPathData;
        public readonly EGridPathAvoidanceStrategy AvoidanceStrategy;

        private List<GridPathElement<TPosition, TContext, TTile>> m_OpenList = new List<GridPathElement<TPosition, TContext, TTile>>();
        private List<GridPathElement<TPosition, TContext, TTile>> m_ClosedList = new List<GridPathElement<TPosition, TContext, TTile>>();
        private TContext m_Context;

        private List<TTile> m_ConnectedList = new List<TTile>();
        private List<GridPathElement<TPosition, TContext, TTile>> m_AvoidanceList = new List<GridPathElement<TPosition, TContext, TTile>>();
        private GridPathElement<TPosition, TContext, TTile> m_FinishElement;

        /// <summary>
        /// Gets the path grid tiles from starting to finishing locations.
        /// </summary>
        /// <remarks>
        /// Returns an empty list on unreachable paths.
        /// </remarks>
        /// <value>
        /// Path grid tiles.
        /// </value>
        public List<GridPathElement<TPosition, TContext, TTile>> Tiles
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
            IGridControl<TPosition, TContext, TTile> i_Grid,
            IGridPathData<TPosition, TContext, TTile> i_PathData,
            TPosition i_StartPos, TPosition i_EndPosition,
            EGridPathAvoidanceStrategy i_AvoidanceStrategy,
            TContext i_Context
        )
        {
            m_Grid = i_Grid;
            m_GridPathData = i_PathData;
            AvoidanceStrategy = i_AvoidanceStrategy;
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
                if ((startElement == m_FinishElement) || (m_FinishElement.PathingState == EGridPathfindingState.Opened))
                {
                    Close(m_FinishElement);
                }
            }
            ProcessAvoidance();
            Finish();
        }

        private GridPathElement<TPosition, TContext, TTile> GetElementOrDefault(TPosition i_Pos)
        {
            GridPathElement<TPosition, TContext, TTile> result;
            if (m_GridPathData.TryGetElement(i_Pos, out result) == EGridPathDataResponse.OutOfDataRange)
            {
                    m_GridPathData.Grow(i_Pos);
                    m_GridPathData.TryGetElement(i_Pos, out result);
            }
            return result;
        }

        private void Open(GridPathElement<TPosition, TContext, TTile> i_Element, GridPathElement<TPosition, TContext, TTile> i_Parent)
        {
            int avoidance;
            float terrainCost = i_Element.Tile.GetCost(m_Grid, i_Parent, m_Context, out avoidance);

            if (terrainCost >= 0.0f)
            {
                switch (AvoidanceStrategy)
                {
                    case EGridPathAvoidanceStrategy.AvoidAll:
                        if (avoidance > 0)
                            return;
                        break;
                    case EGridPathAvoidanceStrategy.LowestCombinedAvoidanceLevel:
                    case EGridPathAvoidanceStrategy.LowestAvoidanceCount:
                        bool needAvoidanceUpdate = false;
                        int newAvoidanceCount = i_Parent.AvoidanceCount;
                        if (avoidance > 0)
                        {
                            newAvoidanceCount++;
                            needAvoidanceUpdate = true;
                            i_Element.AvoidedPathCost = i_Parent.PathCost + terrainCost;
                            i_Element.AvoidanceCount = newAvoidanceCount;
                        }
                        if (i_Parent.AvoidedParent != null)
                        {
                            needAvoidanceUpdate = true;
                            i_Element.AvoidedPathCost = terrainCost + i_Parent.AvoidedPathCost;
                            i_Element.AvoidanceCount = newAvoidanceCount;
                        }

                        if (needAvoidanceUpdate)
                        {
                            i_Element.AvoidanceLevel = avoidance + i_Parent.AvoidanceLevel;
                            i_Element.PathingState = EGridPathfindingState.Opened;
                            i_Element.AvoidedParent = i_Parent;
                            m_AvoidanceList.Add(i_Element);
                            return;
                        }
                        break;
                }

                i_Element.HeuristicDistance = m_Grid.GetHeuristicDistance(i_Element.Tile.Position, m_FinishElement.Tile.Position);
                //cost of the path so far
                i_Element.PathCost = terrainCost + i_Parent.PathCost;
                i_Element.FValue = i_Element.PathCost + i_Element.HeuristicDistance;
                i_Element.PathingState = EGridPathfindingState.Opened;
                i_Element.Parent = i_Parent;
                m_OpenList.Add(i_Element);
            }
        }

        private bool Reopen(GridPathElement<TPosition, TContext, TTile> i_Element, GridPathElement<TPosition, TContext, TTile> i_Parent)
        {
            int avoidance;
            float terrainCost = i_Element.Tile.GetCost(m_Grid, i_Parent, m_Context, out avoidance);

            //negative cost indicates blockers
            if (terrainCost >= 0)
            {
                switch (AvoidanceStrategy)
                {
                    case EGridPathAvoidanceStrategy.AvoidAll:
                        if (avoidance > 0)
                            return false;
                        break;
                    case EGridPathAvoidanceStrategy.LowestCombinedAvoidanceLevel:
                    case EGridPathAvoidanceStrategy.LowestAvoidanceCount:
                        bool needsAvoidanceUpdate = false;
                        int newAvoidanceCount = i_Parent.AvoidanceCount;
                        if (avoidance > 0)
                            newAvoidanceCount++;

                        //is already avoided
                        if (i_Element.AvoidedParent != null)
                        {
                            if (AvoidanceStrategy == EGridPathAvoidanceStrategy.LowestCombinedAvoidanceLevel)
                            {
                                if (i_Element.AvoidanceLevel > (i_Parent.AvoidanceLevel + avoidance))
                                    needsAvoidanceUpdate = true;
                                else
                                    return false;
                            }
                            else
                            {
                                if (i_Element.AvoidanceCount > newAvoidanceCount)
                                    needsAvoidanceUpdate = true;
                                else
                                    return false;
                            }
                        }
                        //is used for avoided path or is now avoided from this new source direction
                        else if (i_Parent.AvoidedParent != null || avoidance > 0)
                        {
                            if (i_Element.PathCost > (terrainCost + i_Parent.PathCost))
                            {
                                needsAvoidanceUpdate = true;
                            }
                            else
                                return false;
                        }

                        if (needsAvoidanceUpdate)
                        {
                            i_Element.AvoidanceCount = newAvoidanceCount;
                            if (i_Parent.AvoidedParent != null)
                                i_Element.AvoidedPathCost = i_Parent.AvoidedPathCost + terrainCost;
                            else
                                i_Element.AvoidedPathCost = i_Parent.PathCost + terrainCost;

                            //only add to the AvoidanceList if it's not going to be there already
                            if (i_Element.PathingState == EGridPathfindingState.Closed || i_Element.AvoidedParent == null)
                            {
                                i_Element.PathingState = EGridPathfindingState.Opened;
                                m_AvoidanceList.Add(i_Element);
                            }
                            i_Element.AvoidanceLevel = i_Parent.AvoidanceLevel + avoidance;
                            i_Element.AvoidedParent = i_Parent;
                            return true;
                        }
                        break;
                }

                float newPathCost = (terrainCost + i_Parent.PathCost);
                if (newPathCost < i_Element.PathCost)
                {
                    i_Element.PathCost = newPathCost;
                    i_Element.FValue = newPathCost + i_Element.HeuristicDistance;
                    i_Element.Parent = i_Parent;
                    return true;
                }
            }
            return false;
        }

        private void Close(GridPathElement<TPosition, TContext, TTile> i_Element)
        {
            i_Element.PathingState = EGridPathfindingState.Closed;
            m_ClosedList.Add(i_Element);
        }

        private void OpenNeighbours(GridPathElement<TPosition, TContext, TTile> i_Element)
        {
            m_Grid.GetConnected(i_Element.Tile.Position, m_ConnectedList);

            int openListSizeBefore = m_OpenList.Count;
            bool oldElementChanged = false;

            int size = m_ConnectedList.Count;
            GridPathElement<TPosition, TContext, TTile> neighbourElement;
            for (var i = 0; i < size; ++i)
            {
                neighbourElement = GetElementOrDefault(m_ConnectedList[i].Position);
                if (neighbourElement != null)
                {
                    switch (neighbourElement.PathingState)
                    {
                        case EGridPathfindingState.New:
                            Open(neighbourElement, i_Element);
                            break;
                        case EGridPathfindingState.Opened:
                            if (i_Element.Parent != neighbourElement)
                            {
                                oldElementChanged = Reopen(neighbourElement, i_Element) || oldElementChanged;
                            }
                            break;
                        case EGridPathfindingState.Closed:
                            if (i_Element.AvoidedParent != null)
                            {
                                oldElementChanged = Reopen(neighbourElement, i_Element) || oldElementChanged;
                            }
                            break;
                    }
                }
            }
            if (oldElementChanged || (openListSizeBefore != m_OpenList.Count))
            {
                m_OpenList.InsertionSort(GridPathElement<TPosition, TContext, TTile>.FValueComparer.Descending);
            }
            m_ConnectedList.Clear();
        }

        private void Finish()
        {
            //clean unused elements
            m_OpenList.Clear();

            int maxPathTileCount = m_ClosedList.Count;
            //if finish exists and was reached
            if ((m_FinishElement != null) && (m_FinishElement.PathingState == EGridPathfindingState.Closed))
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
            m_ClosedList.Clear();
            m_ClosedList = null;

            m_FinishElement = null;
            m_GridPathData.Clean();
            m_GridPathData = null;
        }

        /// <summary>
        /// Filling OpenList with copies of the path elements in the right order.
        /// Copies are made and parent relationship remapped to new elements because
        /// original elements will be recycled to the object pool.
        /// </summary>
        /// <param name="i_Element">Element to backtrack from.</param>
        /// <param name="i_Counter">Backtrack counter.</param>
        /// <returns></returns>
        private GridPathElement<TPosition, TContext, TTile> FillPathResult(GridPathElement<TPosition, TContext, TTile> i_Element, int i_Counter)
        {
            if(i_Element != null)
            {
                var inserted = FillPathResult(i_Element.Parent, ++i_Counter);
                var resultElement = m_OpenList[m_OpenList.Count - i_Counter];
                resultElement.Set(i_Element);
                resultElement.Parent = inserted;
                
                return resultElement;
            }
            else
            {
                GridPathElementPool<TPosition, TContext, TTile>.GLOBAL.GetMultiple(i_Counter, m_OpenList);
            }
            return null;
        }

        private GridPathElement<TPosition, TContext, TTile> PickNext()
        {
            int lastIndex = m_OpenList.Count - 1;
            GridPathElement<TPosition, TContext, TTile> pick = null;
            if (lastIndex >= 0)
            {
                pick = m_OpenList[lastIndex];
                m_OpenList.RemoveAt(lastIndex);
            }

            return pick;
        }

        private void ProcessAvoidance()
        {
            int OpenListSize = m_OpenList.Count;
            int lastIndex = m_AvoidanceList.Count - 1;
            if (AvoidanceStrategy == EGridPathAvoidanceStrategy.LowestAvoidanceCount)
                m_AvoidanceList.InsertionSort(GridPathElement<TPosition, TContext, TTile>.AvoidanceCountComparer.Descending);
            else
                m_AvoidanceList.InsertionSort(GridPathElement<TPosition, TContext, TTile>.AvoidanceLevelComparer.Descending);

            while (lastIndex >= 0 && (m_AvoidanceList[lastIndex] != m_FinishElement))
            {
                var element = m_AvoidanceList[lastIndex];
                m_AvoidanceList.RemoveAt(lastIndex);
                OpenNeighbours(element);
                element.PathingState = EGridPathfindingState.AvoidanceClosed;
                //resort if new elements were added
                if (lastIndex <= m_AvoidanceList.Count)
                {
                    if (AvoidanceStrategy == EGridPathAvoidanceStrategy.LowestAvoidanceCount)
                        m_AvoidanceList.InsertionSort(GridPathElement<TPosition, TContext, TTile>.AvoidanceCountComparer.Descending);
                    else
                        m_AvoidanceList.InsertionSort(GridPathElement<TPosition, TContext, TTile>.AvoidanceLevelComparer.Descending);
                }
                lastIndex = m_AvoidanceList.Count - 1;
            }
            m_AvoidanceList.Clear();

            Log.DebugAssert(m_OpenList.Count != OpenListSize, "Something is generating regular open list during avoidance processing!");
        }

        public void Dispose()
        {
            if (m_OpenList != null)
            {
                GridPathElementPool<TPosition, TContext, TTile>.GLOBAL.RecycleMultiple(m_OpenList);
                m_OpenList = null;
            }
        }
    }
}
