using System;
using System.Collections.Generic;

namespace Common.Grid.Path.Specializations
{
    public class GridPathData2D<TTileData, TContext> : IGridPathData<GridPosition2D, TTileData, TContext>
    {
        private List<GridPathElement<GridPosition2D, TTileData, TContext>> m_Data = new List<GridPathElement<GridPosition2D, TTileData, TContext>>();
        private GridPosition2D m_Min = new GridPosition2D();
        private GridPosition2D m_Max = new GridPosition2D();
        private IGridControl<GridPosition2D, TTileData, TContext> m_Source;
        private readonly GridPathDataProvider<GridPosition2D, TTileData, TContext> m_Origin;

        public GridPathData2D(GridPathDataProvider<GridPosition2D, TTileData, TContext> i_Origin)
        {
            Log.DebugAssert(i_Origin != null, "GridPathData2D constructed with no origin");
            m_Origin = i_Origin;
        }

        public bool Set(IGridControl<GridPosition2D, TTileData, TContext> i_Source, GridPosition2D i_Min, GridPosition2D i_Max)
        {
            m_Source = i_Source;
            m_Min = i_Min;
            m_Max = i_Min;

            if (m_Data.Count < 1)
            {
                m_Data.Add(new GridPathElement<GridPosition2D, TTileData, TContext>());
            }
            GridTile<GridPosition2D, TTileData, TContext> tile;
            if (m_Source.TryGetTile(i_Min, out tile))
            {
                m_Data[0].Tile = tile;

                return Grow(i_Max);
            }
            return false;
        }

        public bool Grow(GridPosition2D i_EnvelopPos)
        {
            GridTile<GridPosition2D, TTileData, TContext> tile;
            if (m_Source.TryGetTile(i_EnvelopPos, out tile))
            {
                GridPosition2D newMin = new GridPosition2D(
                        Math.Min(m_Min.X, i_EnvelopPos.X),
                        Math.Min(m_Min.Y, i_EnvelopPos.Y)
                    );
                GridPosition2D newMax = new GridPosition2D(
                        Math.Max(m_Max.X, i_EnvelopPos.X),
                        Math.Max(m_Max.Y, i_EnvelopPos.Y)
                    );

                if (!m_Min.Equals(newMin) || !m_Max.Equals(newMax))
                {
                    int newSizeX = newMax.X - newMin.X + 1;
                    int newSizeY = newMax.Y - newMin.Y + 1;
                    int oldSizeX = m_Max.X - m_Min.X + 1;
                    int oldSizeY = m_Max.Y - m_Min.Y + 1;
                    int newDataCount = newSizeX * newSizeY;
                    int oldDataCount = oldSizeX * oldSizeY;
                    var oldData = new List<GridPathElement<GridPosition2D, TTileData, TContext>>(oldDataCount);

                    //fill in required new data elements
                    if (m_Data.Capacity < newDataCount)
                    {
                        m_Data.Capacity = newDataCount;
                    }
                    for (int i = m_Data.Count; i < newDataCount; ++i)
                    {
                        m_Data.Add(new GridPathElement<GridPosition2D, TTileData, TContext>());
                    }
                    //record old data to map elements into a new position afterwards
                    for (int i = 0; i < oldDataCount; ++i)
                    {
                        oldData.Add(m_Data[i]);
                    }

                    int newStrideX = newSizeY;
                    int oldStrideX = oldSizeY;
                    //indicater for what index to take new data elements from 
                    int freeElementIndex = oldDataCount;
                    for (int itX = newMin.X; itX <= newMax.X; ++itX)
                    {
                        bool inOldXRange = m_Min.X <= itX && m_Max.X >= itX;
                        for (int itY = newMin.Y; itY <= newMax.Y; ++itY)
                        {
                            int newIndex = newStrideX * (itX - newMin.X) + (itY - newMin.X);
                            //if is old data
                            if (inOldXRange && m_Min.Y <= itY && m_Max.Y >= itY)
                            {
                                int oldIndex = oldStrideX * (itX - m_Min.X) + (itY - m_Min.Y);
                                m_Data[newIndex] = oldData[oldIndex];
                            }
                            else
                            {
                                var newElement = m_Data[freeElementIndex++];
                                GridTile<GridPosition2D, TTileData, TContext> tempTile;
                                m_Source.TryGetTile(new GridPosition2D(itX, itY), out tempTile);
                                newElement.Tile = tempTile;
                                m_Data[newIndex] = newElement;
                            }
                        }
                    }
                    
                    m_Min = newMin;
                    m_Max = newMax;
                }
                return true;
            }
            return false;
        }

        public GridPathDataResponse TryGetElement(GridPosition2D i_Pos, out GridPathElement<GridPosition2D, TTileData, TContext> o_Value)
        {
            if (m_Min.X <= i_Pos.X && m_Max.X >= i_Pos.X && m_Min.Y <= i_Pos.Y && m_Max.Y >= i_Pos.Y)
            {
                int strideX = m_Max.Y - m_Min.Y + 1;
                int index = strideX * (i_Pos.X - m_Min.X) + (i_Pos.Y - m_Min.Y);
                o_Value = m_Data[index];
                return GridPathDataResponse.Success;
            }

            o_Value = null;
            GridTile<GridPosition2D, TTileData, TContext> tile;
            if (m_Source.TryGetTile(i_Pos, out tile))
            {
                return GridPathDataResponse.OutOfDataRange;
            }
            return GridPathDataResponse.InvalidPosition;
        }

        public void Dispose()
        {
            m_Origin.Recycle(this);
        }

        public void Clean()
        {
            int count = m_Data.Count;
            for (int i = 0; i < count; ++i)
            {
                m_Data[i].Clear();
            }
        }
    }
}
