using System;
using System.Collections.Generic;

namespace Common.Grid.Path
{
    public abstract class GridPathDataProvider<TPosition, TTileData, TContext>
    {
        private readonly object m_SyncLock = new object();
        private readonly List<IGridPathData<TPosition, TTileData, TContext>> m_Data = new List<IGridPathData<TPosition, TTileData, TContext>>();

        public IGridPathData<TPosition, TTileData, TContext> GetGridPathData()
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

        protected abstract IGridPathData<TPosition, TTileData, TContext> Create();

        public void Recycle(IGridPathData<TPosition, TTileData, TContext> i_Data)
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
