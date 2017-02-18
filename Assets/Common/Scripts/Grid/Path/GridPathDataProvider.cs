using System;
using System.Collections.Generic;

namespace Common.Grid.Path
{
    public abstract class GridPathDataProvider<TTile, TTerrain, TPosition, TContext> : IGridPathDataProvider<TTile, TTerrain, TPosition, TContext>
        where TTile : GridTile<TTerrain, TPosition, TContext>
        where TTerrain : GridTerrain<TContext>
        where TContext : IGridContext<TTile, TTerrain, TPosition, TContext>
    {
        private readonly object m_SyncLock = new object();
        private readonly List<IGridPathData<TTile, TTerrain, TPosition, TContext>> m_Data = new List<IGridPathData<TTile, TTerrain, TPosition, TContext>>();

        public IGridPathData<TTile, TTerrain, TPosition, TContext> GetGridPathData()
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
            return Create();
        }

        protected abstract IGridPathData<TTile, TTerrain, TPosition, TContext> Create();

        public void Recycle(IGridPathData<TTile, TTerrain, TPosition, TContext> i_Data)
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
    }
}
