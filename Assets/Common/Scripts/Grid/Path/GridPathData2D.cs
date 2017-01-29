using System;
using System.Collections.Generic;

namespace Common.Grid.Path
{
    public class GridPathData2D<TTile, TTerrain, TContext> : IGridPathData<TTile, TTerrain, GridPosition2D, TContext>
        where TTile : GridTile<TTerrain, GridPosition2D, TContext>
        where TTerrain : GridTerrain<TContext>
    {
        private List<GridPathElement<TTile, TTerrain, GridPosition2D, TContext>> m_Data = new List<GridPathElement<TTile, TTerrain, GridPosition2D, TContext>>();
        private GridPosition2D m_Min = new GridPosition2D();
        private GridPosition2D m_Max = new GridPosition2D();
        private int m_SizeX;
        private IGridControl<TTile, TTerrain, GridPosition2D, TContext> m_Source;
        private readonly IGridPathDataProvider<TTile, TTerrain, GridPosition2D, TContext> m_Origin;

        public GridPathData2D(IGridPathDataProvider<TTile, TTerrain, GridPosition2D, TContext> i_Origin)
        {
            Log.DebugAssert(i_Origin != null, "GridPathData2D constructed with no origin");
            m_Origin = i_Origin;
        }

        public void Set(IGridControl<TTile, TTerrain, GridPosition2D, TContext> i_Source, GridPosition2D i_Min, GridPosition2D i_Max)
        {
            m_Source = i_Source;
            m_Min = i_Min;
            m_Max = i_Min;

            if (m_Data.Count < 1)
            {
                m_Data.Add(new GridPathElement<TTile, TTerrain, GridPosition2D, TContext>());
            }
            m_Data[0].Tile = m_Source.GetTile(i_Min);

            Grow(i_Max);
        }

        public bool Grow(GridPosition2D i_EnvelopPos)
        {
            TTile tile;
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
                    int newSizeX = newMax.X - newMin.X;
                    int newDataCount = newSizeX * (newMax.Y - newMin.Y);
                    int oldDataCount = m_SizeX * (m_Max.Y - m_Min.Y);
                    var oldData = new List<GridPathElement<TTile, TTerrain, GridPosition2D, TContext>>(oldDataCount);

                    //fill in required new data elements
                    if (m_Data.Capacity < newDataCount)
                    {
                        m_Data.Capacity = newDataCount;
                    }
                    for (int i = m_Data.Count; i < newDataCount; ++i)
                    {
                        m_Data.Add(new GridPathElement<TTile, TTerrain, GridPosition2D, TContext>());
                    }

                    for (int i = 0; i < oldDataCount; ++i)
                    {
                        oldData.Add(m_Data[i]);
                    }

                    int freeElementIndex = oldDataCount;
                    for (int itY = newMin.Y; itY < newMax.Y; ++itY)
                    {
                        int indexYPart = (itY - newMin.Y) * newSizeX;
                        int oldIndexYPart = (itY - m_Min.Y) * m_SizeX;

                        for (int itX = newMin.X; itX < newMax.X; ++itX)
                        {
                            int index = indexYPart + itX - newMin.X;

                            if (m_Min.X <= itX && m_Min.Y <= itY && m_Max.X >= itX && m_Max.Y >= itY)
                            {
                                int oldIndex = oldIndexYPart + itX - m_Min.X;
                                m_Data[index] = oldData[oldIndex];
                            }
                            else
                            {
                                var temp = m_Data[freeElementIndex++];
                                temp.Tile = m_Source.GetTile(new GridPosition2D(itX, itY));
                                m_Data[index] = temp;
                            }
                        }
                    }

                    
                    m_Min = newMin;
                    m_Max = newMax;
                    m_SizeX = newSizeX;
                }
                return true;
            }
            return false;
        }

        public GridPathDataResponse TryGetElement(GridPosition2D i_Pos, out GridPathElement<TTile, TTerrain, GridPosition2D, TContext> o_Value)
        {
            if (m_Min.X < i_Pos.X && m_Min.Y < i_Pos.Y && m_Max.X > i_Pos.X && m_Max.Y > i_Pos.Y)
            {
                int index = (i_Pos.Y - m_Min.Y) * m_SizeX + i_Pos.X - m_Min.X;
                o_Value = m_Data[index];
                return GridPathDataResponse.Success;
            }

            o_Value = null;
            TTile tile;
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
