﻿using System;
using System.Collections.Generic;

namespace Common.Grid.Path.Specializations
{
    /// <summary>
    /// A specialization of <see cref="IGridPathData{TPosition, TContext, TTile}"/> for 3D grid pathing data management.
    /// </summary>
    /// <typeparam name="TContext">Scenario specific context information for processing data.</typeparam>
    public class GridPathData3D<TContext, TTile> : IGridPathData<GridPosition3D, TContext, TTile> where TTile : GridTile<GridPosition3D, TContext, TTile>
    {
        private List<GridPathElement<GridPosition3D, TContext, TTile>> m_Data = new List<GridPathElement<GridPosition3D, TContext, TTile>>();
        private GridPosition3D m_Min = new GridPosition3D();
        private GridPosition3D m_Max = new GridPosition3D(-1, -1, -1);
        private IGridControl<GridPosition3D, TContext, TTile> m_Source;
        private readonly GridPathDataProvider<GridPosition3D, TContext, TTile> m_Origin;

        public GridPathData3D(GridPathDataProvider<GridPosition3D, TContext, TTile> i_Origin)
        {
            Log.DebugAssert(i_Origin != null, "GridPathData3D constructed with no origin");
            m_Origin = i_Origin;
        }

        public GridPosition3D GetMin()
        {
            return m_Min;
        }

        public GridPosition3D GetMax()
        {
            return m_Max;
        }

        public bool Set(IGridControl<GridPosition3D, TContext, TTile> i_Source, GridPosition3D i_Min, GridPosition3D i_Max)
        {
            m_Source = i_Source;
            m_Min = i_Min;
            m_Max = i_Min;

            if (m_Data.Count < 1)
            {
                m_Data.Add(GridPathElementPool<GridPosition3D, TContext, TTile>.GLOBAL.Get());
            }
            TTile tile;
            if (m_Source.TryGetTile(i_Min, out tile))
            {
                m_Data[0].Tile = tile;

                return Grow(i_Max);
            }
            return false;
        }

        public bool Grow(GridPosition3D i_EnvelopPos)
        {
            TTile tile;
            if (m_Source.TryGetTile(i_EnvelopPos, out tile))
            {
                GridPosition3D newMin = new GridPosition3D(
                        Math.Min(m_Min.X, i_EnvelopPos.X),
                        Math.Min(m_Min.Y, i_EnvelopPos.Y),
                        Math.Min(m_Min.Z, i_EnvelopPos.Z)
                    );
                GridPosition3D newMax = new GridPosition3D(
                        Math.Max(m_Max.X, i_EnvelopPos.X),
                        Math.Max(m_Max.Y, i_EnvelopPos.Y),
                        Math.Max(m_Max.Z, i_EnvelopPos.Z)
                    );

                if (!m_Min.Equals(newMin) || !m_Max.Equals(newMax))
                {
                    int newSizeX = newMax.X - newMin.X + 1;
                    int newSizeY = newMax.Y - newMin.Y + 1;
                    int newSizeZ = newMax.Z - newMin.Z + 1;
                    int oldSizeX = m_Max.X - m_Min.X + 1;
                    int oldSizeY = m_Max.Y - m_Min.Y + 1;
                    int oldSizeZ = m_Max.Z - m_Min.Z + 1;
                    int newDataCount = newSizeX * newSizeY * newSizeZ;
                    int freeElementIndex = oldSizeX * oldSizeY * oldSizeZ;
                    int addedDataCount = newDataCount - m_Data.Count;
                    var oldData = new List<GridPathElement<GridPosition3D, TContext, TTile>>(m_Data);

                    //fill in required new data elements
                    if (m_Data.Capacity < newDataCount)
                    {
                        m_Data.Capacity = newDataCount;
                    }
                    if (addedDataCount > 0)
                    {
                        GridPathElementPool<GridPosition3D, TContext, TTile>.GLOBAL.GetMultiple(addedDataCount, m_Data);
                    }

                    int newStrideX = newSizeY * newSizeZ;
                    int newStrideY = newSizeZ;
                    int oldStrideX = oldSizeY * oldSizeZ;
                    int oldStrideY = oldSizeZ;
                    //indicater for what index to take new data elements from 
                    for (int itX = newMin.X; itX <= newMax.X; ++itX)
                    {
                        bool inOldXRange = m_Min.X <= itX && m_Max.X >= itX;
                        for (int itY = newMin.Y; itY <= newMax.Y; ++itY)
                        {
                            bool inOldXYRange = inOldXRange && m_Min.Y <= itY && m_Max.Y >= itY;
                            for (int itZ = newMin.Z; itZ <= newMax.Z; ++itZ)
                            {
                                int newIndex = newStrideX * (itX - newMin.X) + newStrideY * (itY - newMin.Y) + (itZ - newMin.Z);
                                //if is old data
                                if (inOldXYRange && m_Min.Z <= itZ && m_Max.Z >= itZ)
                                {
                                    int oldIndex = oldStrideX * (itX - m_Min.X) + oldStrideY * (itY - m_Min.Y) + (itZ - m_Min.Z);
                                    m_Data[newIndex] = oldData[oldIndex];
                                }
                                else
                                {
                                    var newElement = m_Data[freeElementIndex++];
                                    m_Source.TryGetTile(new GridPosition3D(itX, itY, itZ), out newElement.Tile);
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

        public EGridPathDataResponse TryGetElement(GridPosition3D i_Pos, out GridPathElement<GridPosition3D, TContext, TTile> o_Value)
        {
            if (m_Min.X <= i_Pos.X && m_Max.X >= i_Pos.X && m_Min.Y <= i_Pos.Y && m_Max.Y >= i_Pos.Y && m_Min.Z <= i_Pos.Z && m_Max.Z >= i_Pos.Z)
            {
                int strideX = (m_Max.Y - m_Min.Y + 1) * (m_Max.Z - m_Min.Z + 1);
                int strideY = (m_Max.Z - m_Min.Z + 1);
                int index = strideX * (i_Pos.X - m_Min.X) + strideY * (i_Pos.Y - m_Min.Y) + (i_Pos.Z - m_Min.Z);
                o_Value = m_Data[index];
                return EGridPathDataResponse.Success;
            }

            o_Value = null;
            TTile tile;
            if (m_Source.TryGetTile(i_Pos, out tile))
            {
                return EGridPathDataResponse.OutOfDataRange;
            }
            return EGridPathDataResponse.InvalidPosition;
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
                m_Data[i].PoolingClear();
            }
        }

        public void OnDestroy()
        {
            if (m_Data != null)
            {
                GridPathElementPool<GridPosition3D, TContext, TTile>.GLOBAL.RecycleMultiple(m_Data);
                m_Data = null;
            }
        }
    }
}
