using System;
using System.Collections.Generic;

namespace Common.Grid.Path
{
    /// <summary>
    /// A pool of reusable <see cref="GridPathElement{TPosition, TTileData, TContext}"/> instances.
    /// </summary>
    public class GridPathElementPool<TPosition, TTileData, TContext>
    {
        private readonly object m_SyncLock = new object();
        private readonly List<GridPathElement<TPosition, TTileData, TContext>> m_Data;

        public GridPathElementPool(int i_Capacity)
        {
            m_Data = new List<GridPathElement<TPosition, TTileData, TContext>>(i_Capacity);
        }

        public GridPathElement<TPosition, TTileData, TContext> Get()
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
            return new GridPathElement<TPosition, TTileData, TContext>();
        }

        public void GetMultiple(int i_Count, List<GridPathElement<TPosition, TTileData, TContext>> o_List)
        {
            if (o_List != null)
            {
                int newCapacity = o_List.Count + i_Count;
                if(o_List.Capacity < newCapacity)
                {
                    o_List.Capacity = newCapacity;
                }

                int filledCount;
                lock (m_SyncLock)
                {
                    int dataCount = m_Data.Count;
                    int minIndex = Math.Max(dataCount - i_Count, 0);
                    filledCount = dataCount - minIndex;
                    if(filledCount > 0)
                    {
                        for(int i = minIndex; i < dataCount; ++i)
                        {
                            o_List.Add(m_Data[i]);
                        }
                        m_Data.RemoveRange(minIndex, filledCount);
                    }
                }
                
                while (filledCount < i_Count)
                {
                    o_List.Add(new GridPathElement<TPosition, TTileData, TContext>());
                    ++filledCount;
                }
            }
        }

        public void Recycle(GridPathElement<TPosition, TTileData, TContext> i_Data)
        {
            i_Data.Clear();
            lock (m_SyncLock)
            {
                if (m_Data.Count < m_Data.Capacity)
                {
                    m_Data.Add(i_Data);
                }
            }
        }

        public void RecycleMultiple(List<GridPathElement<TPosition, TTileData, TContext>> i_Data)
        {
            int newDataCount = i_Data.Count;
            lock (m_SyncLock)
            {
                int count = Math.Min(m_Data.Capacity - m_Data.Count, newDataCount);
                for (int i = 0; i < count; ++i)
                {
                    var element = i_Data[i];
                    element.Clear();
                    m_Data.Add(element);
                }
            }
            i_Data.Clear();
        }

        public static readonly GridPathElementPool<TPosition, TTileData, TContext> GLOBAL = new GridPathElementPool<TPosition, TTileData, TContext>(200);
    }
}
