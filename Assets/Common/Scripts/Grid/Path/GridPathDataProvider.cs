using System;
using System.Collections.Generic;

namespace Common.Grid.Path
{
    /// <summary>
    /// A basic class for implementing a provider of <see cref="IGridPathData{TPosition, TTileData, TContext}"/> data.
    /// </summary>
    /// <typeparam name="TPosition">The type of the position.</typeparam>
    /// <typeparam name="TTileData">The type of the tile data.</typeparam>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    public abstract class GridPathDataProvider<TPosition, TTileData, TContext>
    {
        private const int DEFAULT_PATH_DATA_CAPACITY = 5;
        private readonly object m_SyncLock = new object();
        private readonly List<IGridPathData<TPosition, TTileData, TContext>> m_Data = new List<IGridPathData<TPosition, TTileData, TContext>>(DEFAULT_PATH_DATA_CAPACITY);

        public IGridPathData<TPosition, TTileData, TContext> GetGridPathData()
        {
            lock (m_SyncLock)
            {
                if (m_Data.Count > 0)
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
                if (!m_Data.Contains(i_Data))
                {
                    if (m_Data.Count >= m_Data.Capacity)
                    {
                        i_Data.OnDestroy();
                    }
                    else
                    {
                        m_Data.Add(i_Data);
                    }
                }
            }
        }

        public void SetCapacity(int i_Capacity)
        {
            i_Capacity = Math.Max(i_Capacity, 1);
            if (m_Data.Count > i_Capacity)
            {
                lock (m_SyncLock)
                {
                    for(int i = m_Data.Count - 1; i >= i_Capacity; --i)
                    {
                        m_Data[i].OnDestroy();
                    }
                    m_Data.RemoveRange(i_Capacity, m_Data.Count - i_Capacity);
                    m_Data.Capacity = i_Capacity;
                }
            }
            else
            {
                if(m_Data.Capacity != i_Capacity)
                {
                    lock (m_SyncLock)
                    {
                        m_Data.Capacity = i_Capacity;
                    }
                }
            }
        }
    }
}
