using System;
using System.Collections.Generic;
using Common.Collections;

namespace Common.Grid.Path
{
    /// <summary>
    /// A grid pathfinding scanner from a given origin position for a specified area.
    /// Introduced to help AI understand the available movement in an area.
    /// </summary>
    /// <typeparam name="TPosition">The type of the position.</typeparam>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    /// <typeparam name="TTile">The type of the tile.</typeparam>
    /// <seealso cref="System.IDisposable" />
    public class GridPathArea<TPosition, TContext, TTile> : IDisposable where TTile : GridTile<TPosition, TContext, TTile>
    {
        public readonly IGridControl<TPosition, TContext, TTile> Grid;
        public readonly IGridPathData<TPosition, TContext, TTile> GridPathData;
        public readonly EGridPathAvoidanceStrategy AvoidanceStrategy;
        public readonly TPosition Origin;

        private TContext m_Context;
        private readonly Queue<GridPathElement<TPosition, TContext, TTile>> m_OpenQueue = new Queue<GridPathElement<TPosition, TContext, TTile>>();
        private readonly List<TTile> m_ConnectedList = new List<TTile>();
        private readonly List<GridPathElement<TPosition, TContext, TTile>> AvoidanceList = new List<GridPathElement<TPosition, TContext, TTile>>();

        public GridPathArea(
            IGridControl<TPosition, TContext, TTile> i_Grid,
            IGridPathData<TPosition, TContext, TTile> i_PathData,
            TPosition i_Origin, EGridPathAvoidanceStrategy i_AvoidanceStrategy,
            TContext i_Context
        )
        {
            Grid = i_Grid;
            GridPathData = i_PathData;
            AvoidanceStrategy = i_AvoidanceStrategy;
            Origin = i_Origin;
            m_Context = i_Context;

            GridPathElement<TPosition, TContext, TTile> originElement;
            if (i_PathData.TryGetElement(i_Origin, out originElement) == EGridPathDataResponse.Success)
            {
                originElement.PathingState = EGridPathfindingState.Opened;
                m_OpenQueue.Enqueue(originElement);

                OpenNeighbours(originElement);
                while (m_OpenQueue.Count > 0)
                {
                    OpenNeighbours(m_OpenQueue.Dequeue());
                }

                ProcessAvoidance();
            }
            m_OpenQueue = null;
            m_ConnectedList = null;
        }

        private void Open(GridPathElement<TPosition, TContext, TTile> i_Element, GridPathElement<TPosition, TContext, TTile> i_Parent)
        {
            int avoidance;
            float terrainCost = i_Element.Tile.GetCost(Grid, i_Parent, m_Context, out avoidance);

            //negative cost indicates not traversable terrain
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
                            AvoidanceList.Add(i_Element);
                            return;
                        }
                        break;
                }

                m_OpenQueue.Enqueue(i_Element);
                //cost of the path so far
                i_Element.PathCost = terrainCost + i_Parent.PathCost;
                //will scan the whole area so no need for fValue
                //i_Element.FValue = i_Element.PathCost + i_Element.HeuristicDistance;
                i_Element.PathingState = EGridPathfindingState.Opened;
                i_Element.Parent = i_Parent;
            }
        }

        private bool Reopen(GridPathElement<TPosition, TContext, TTile> i_Element, GridPathElement<TPosition, TContext, TTile> i_Parent)
        {
            int avoidance;
            float terrainCost = i_Element.Tile.GetCost(Grid, i_Parent, m_Context, out avoidance);

            //negative cost indicates not traversable terrain
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
                                AvoidanceList.Add(i_Element);
                            }
                            i_Element.AvoidanceLevel = i_Parent.AvoidanceLevel + avoidance;
                            i_Element.AvoidedParent = i_Parent;
                            return true;
                        }
                        break;
                }

                //Regular pathing calculations
                float newPathCost = (terrainCost + i_Parent.PathCost);
                if (newPathCost < i_Element.PathCost)
                {
                    i_Element.PathCost = newPathCost;
                    //will scan the whole area so no need for fValue
                    //i_Element.FValue = newPathCost + i_Element.HeuristicDistance;
                    i_Element.Parent = i_Parent;
                    return true;
                }
            }
            return false;
        }

        private void OpenNeighbours(GridPathElement<TPosition, TContext, TTile> i_Element)
        {
            Grid.GetConnected(i_Element.Tile.Position, m_ConnectedList);
            int size = m_ConnectedList.Count;
            for (var i = 0; i < size; ++i)
            {
                GridPathElement<TPosition, TContext, TTile> neighbourElement;
                //only continue if position is within GridPathData area 
                if (GridPathData.TryGetElement(m_ConnectedList[i].Position, out neighbourElement) == EGridPathDataResponse.Success)
                {
                    switch (neighbourElement.PathingState)
                    {
                        case EGridPathfindingState.New:
                            Open(neighbourElement, i_Element);
                            break;
                        case EGridPathfindingState.Opened:
                            if (i_Element.Parent != neighbourElement)
                            {
                                Reopen(neighbourElement, i_Element);
                            }
                            break;
                        case EGridPathfindingState.Closed:
                            if (i_Element.AvoidedParent != null)
                            {
                                Reopen(neighbourElement, i_Element);
                            }
                            break;
                    }
                }
            }
            m_ConnectedList.Clear();
        }

        private void ProcessAvoidance()
        {
            int lastIndex = AvoidanceList.Count - 1;
            if (AvoidanceStrategy == EGridPathAvoidanceStrategy.LowestAvoidanceCount)
                AvoidanceList.InsertionSort(GridPathElement<TPosition, TContext, TTile>.AvoidanceCountComparer.Descending);
            else
                AvoidanceList.InsertionSort(GridPathElement<TPosition, TContext, TTile>.AvoidanceLevelComparer.Descending);

            while (lastIndex >= 0)
            {
                var element = AvoidanceList[lastIndex];
                AvoidanceList.RemoveAt(lastIndex);
                OpenNeighbours(element);
                element.PathingState = EGridPathfindingState.AvoidanceClosed;
                //resort if new elements were added
                if (lastIndex <= AvoidanceList.Count)
                {
                    if (AvoidanceStrategy == EGridPathAvoidanceStrategy.LowestAvoidanceCount)
                        AvoidanceList.InsertionSort(GridPathElement<TPosition, TContext, TTile>.AvoidanceCountComparer.Descending);
                    else
                        AvoidanceList.InsertionSort(GridPathElement<TPosition, TContext, TTile>.AvoidanceLevelComparer.Descending);
                }
                lastIndex = AvoidanceList.Count - 1;
            }

            Log.DebugAssert(m_OpenQueue.Count <= 0, "Something is generating regular open list during avoidance processing!");
        }

        public void Dispose()
        {
            GridPathData.Dispose();
        }
    }
}
