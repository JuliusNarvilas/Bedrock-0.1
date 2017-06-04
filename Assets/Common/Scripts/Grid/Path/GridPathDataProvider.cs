using System;
using System.Collections.Generic;

namespace Common.Grid.Path
{
    /// <summary>
    /// A basic class for implementing a provider of <see cref="IGridPathData{TPosition, TTileData, TContext}"/> data.
    /// </summary>
    /// <typeparam name="TPosition">The type of the position.</typeparam>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    public abstract class GridPathDataProvider<TPosition, TContext, TTile> where TTile : GridTile<TPosition, TContext, TTile>
    {
        private const int DEFAULT_PATH_DATA_CAPACITY = 5;
        private readonly object m_SyncLock = new object();
        private readonly List<IGridPathData<TPosition, TContext, TTile>> m_Data = new List<IGridPathData<TPosition, TContext, TTile>>(DEFAULT_PATH_DATA_CAPACITY);

        public IGridPathData<TPosition, TContext, TTile> GetGridPathData()
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

        protected abstract IGridPathData<TPosition, TContext, TTile> Create();

        public void Recycle(IGridPathData<TPosition, TContext, TTile> i_Data)
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
