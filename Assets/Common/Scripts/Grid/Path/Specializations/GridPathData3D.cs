using System;
using System.Collections.Generic;

namespace Common.Grid.Path.Specializations
{
    /// <summary>
    /// A specialization of <see cref="IGridPathData{TPosition, TTileData, TContext}"/> for 3D grid pathing data management.
    /// </summary>
    /// <typeparam name="TTileData">Scenario specific tile data type.</typeparam>
    /// <typeparam name="TContext">Scenario specific context information for processing data.</typeparam>
    public class GridPathData3D<TTileData, TContext> : IGridPathData<GridPosition3D, TTileData, TContext>
    {
        private List<GridPathElement<GridPosition3D, TTileData, TContext>> m_Data = new List<GridPathElement<GridPosition3D, TTileData, TContext>>();
        private GridPosition3D m_Min = new GridPosition3D();
        private GridPosition3D m_Max = new GridPosition3D();
        private IGridControl<GridPosition3D, TTileData, TContext> m_Source;
        private readonly GridPathDataProvider<GridPosition3D, TTileData, TContext> m_Origin;

        public GridPathData3D(GridPathDataProvider<GridPosition3D, TTileData, TContext> i_Origin)
        {
            Log.DebugAssert(i_Origin != null, "GridPathData3D constructed with no origin");
            m_Origin = i_Origin;
        }

        public bool Set(IGridControl<GridPosition3D, TTileData, TContext> i_Source, GridPosition3D i_Min, GridPosition3D i_Max)
        {
            m_Source = i_Source;
            m_Min = i_Min;
            m_Max = i_Min;

            if (m_Data.Count < 1)
            {
                m_Data.Add(new GridPathElement<GridPosition3D, TTileData, TContext>());
            }
            GridTile<GridPosition3D, TTileData, TContext> tile;
            if (m_Source.TryGetTile(i_Min, out tile))
            {
                m_Data[0].Tile = tile;

                return Grow(i_Max);
            }
            return false;
        }

        public bool Grow(GridPosition3D i_EnvelopPos)
        {
            GridTile<GridPosition3D, TTileData, TContext> tile;
            if (m_Source.TryGetTile(i_EnvelopPos, out tile))
            {
                GridPosition3D newMin = new GridPosition3D(
                        Math.Min(m_Min.X, i_EnvelopPos.X),
                        Math.Min(m_Min.Y, i_EnvelopPos.Y)
                    );
                GridPosition3D newMax = new GridPosition3D(
                        Math.Max(m_Max.X, i_EnvelopPos.X),
                        Math.Max(m_Max.Y, i_EnvelopPos.Y)
                    );

                if (!m_Min.Equals(newMin) || !m_Max.Equals(newMax))
                {
                    int newSizeX = newMax.X - newMin.X;
                    int newSizeY = newMax.Y - newMin.Y;
                    int newSizeZ = newMax.Z - newMin.Z;
                    int oldSizeX = m_Max.X - m_Min.X;
                    int oldSizeY = m_Max.Y - m_Min.Y;
                    int oldSizeZ = m_Max.Z - m_Min.Z;
                    int newDataCount = newSizeX * newSizeY * newSizeZ;
                    int oldDataCount = oldSizeX * oldSizeY * oldSizeZ;
                    var oldData = new List<GridPathElement<GridPosition3D, TTileData, TContext>>(oldDataCount);

                    //fill in required new data elements
                    if (m_Data.Capacity < newDataCount)
                    {
                        m_Data.Capacity = newDataCount;
                    }
                    for (int i = m_Data.Count; i < newDataCount; ++i)
                    {
                        m_Data.Add(new GridPathElement<GridPosition3D, TTileData, TContext>());
                    }
                    //record old data to map elements into a new position afterwards
                    for (int i = 0; i < oldDataCount; ++i)
                    {
                        oldData.Add(m_Data[i]);
                    }

                    int newStrideX = newSizeY * newSizeZ;
                    int newStrideY = newSizeZ;
                    int oldStrideX = oldSizeY * oldSizeZ;
                    int oldStrideY = oldSizeZ;
                    //indicater for what index to take new data elements from 
                    int freeElementIndex = oldDataCount;
                    for (int itX = newMin.X; itX <= newMax.X; ++itX)
                    {
                        bool inOldXRange = m_Min.X <= itX && m_Max.X >= itX;
                        for (int itY = newMin.Y; itY < newMax.Y; ++itY)
                        {
                            bool inOldYRange = m_Min.Y <= itY && m_Max.Y >= itY;
                            for (int itZ = newMin.Z; itZ <= newMax.Z; ++itZ)
                            {
                                int newIndex = newStrideX * (itX - newMin.X) + newStrideY * (itY - newMin.Y) + (itZ - newMin.Z);
                                //if is old data
                                if (inOldXRange && inOldYRange && m_Min.Z <= itZ && m_Max.Z >= itZ)
                                {
                                    int oldIndex = oldStrideX * (itX - m_Min.X) + oldStrideY * (itY - m_Min.Y) + (itZ - m_Min.Z);
                                    m_Data[newIndex] = oldData[oldIndex];
                                }
                                else
                                {
                                    var newElement = m_Data[freeElementIndex++];
                                    GridTile<GridPosition3D, TTileData, TContext> tempTile;
                                    m_Source.TryGetTile(new GridPosition3D(itX, itY, itZ), out tempTile);
                                    newElement.Tile = tempTile;
                                    m_Data[newIndex] = newElement;
                                }
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

        public GridPathDataResponse TryGetElement(GridPosition3D i_Pos, out GridPathElement<GridPosition3D, TTileData, TContext> o_Value)
        {
            if (m_Min.X <= i_Pos.X && m_Max.X >= i_Pos.X && m_Min.Y <= i_Pos.Y && m_Max.Y >= i_Pos.Y && m_Min.Z <= i_Pos.Z && m_Max.Z >= i_Pos.Z)
            {
                int strideX = (m_Max.Y - m_Min.Y + 1) * (m_Max.Z - m_Min.Z + 1);
                int strideY = (m_Max.Z - m_Min.Z + 1);
                int index = strideX * (i_Pos.X - m_Min.X) + strideY * (i_Pos.Y - m_Min.Y) + (i_Pos.Z - m_Min.Z);
                o_Value = m_Data[index];
                return GridPathDataResponse.Success;
            }

            o_Value = null;
            GridTile<GridPosition3D, TTileData, TContext> tile;
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

        public void OnDestroy()
        {
            if (m_Data != null)
            {
                GridPathElementPool<GridPosition3D, TTileData, TContext>.GLOBAL.RecycleMultiple(m_Data);
                m_Data = null;
            }
        }
    }
}
