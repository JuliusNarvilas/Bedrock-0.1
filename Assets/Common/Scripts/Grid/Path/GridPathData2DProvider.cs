using System;
using System.Collections.Generic;

namespace Common.Grid.Path
{
    public class GridPathData2DProvider<TTile, TTerrain, TContext> : IGridPathDataProvider<TTile, TTerrain, GridPosition2D, TContext>
        where TTile : GridTile<TTerrain, GridPosition2D, TContext>
        where TTerrain : GridTerrain<TContext>
    {
        private readonly object m_SyncLock = new object();
        List<IGridPathData<TTile, TTerrain, GridPosition2D, TContext>> m_Data = new List<IGridPathData<TTile, TTerrain, GridPosition2D, TContext>>();

        public IGridPathData<TTile, TTerrain, GridPosition2D, TContext> GetGridPathData()
        {
            lock(m_SyncLock)
            {
                if(m_Data.Count > 0)
                {
                    var result = m_Data[m_Data.Count - 1];
                    m_Data.RemoveAt(m_Data.Count - 1);
                    return result;
                }
            }
            return new GridPathData2D<TTile, TTerrain, TContext>(this);
        }

        public void Recycle(IGridPathData<TTile, TTerrain, GridPosition2D, TContext> i_Data)
        {
            i_Data.Clean();
            lock (m_SyncLock)
            {
                m_Data.Add(i_Data);
            }
        }

        public void Reduce(int i_Capacity)
        {
            i_Capacity = Math.Max(i_Capacity, 0);
            if (m_Data.Count > i_Capacity)
            {
                lock (m_SyncLock)
                {
                    for(int i = m_Data.Count - 1; i >= i_Capacity; --i)
                    {
                        m_Data.RemoveAt(i);
                    }
                }
            }
        }

        public static readonly GridPathData2DProvider<TTile, TTerrain, TContext> GLOBAL = new GridPathData2DProvider<TTile, TTerrain, TContext>();
    }
}
