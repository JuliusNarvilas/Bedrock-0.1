using System;
using System.Collections.Generic;

namespace Common.Grid.Path
{
    public class GridPathElementPool<TTile, TTerrain, TPosition, TContext>
        where TTile : GridTile<TTerrain, TPosition, TContext>
        where TTerrain : GridTerrain<TContext>
    {
        private readonly object m_SyncLock = new object();
        private readonly List<GridPathElement<TTile, TTerrain, TPosition, TContext>> m_Data = new List<GridPathElement<TTile, TTerrain, TPosition, TContext>>();
        private int m_MaxCapacity;

        public GridPathElementPool(int i_Capacity)
        {
            m_MaxCapacity = i_Capacity;
        }

        public GridPathElement<TTile, TTerrain, TPosition, TContext> Get()
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
            return new GridPathElement<TTile, TTerrain, TPosition, TContext>();
        }

        public void GetMultiple(int i_Count, List<GridPathElement<TTile, TTerrain, TPosition, TContext>> o_List)
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
                    o_List.Add(new GridPathElement<TTile, TTerrain, TPosition, TContext>());
                    ++filledCount;
                }
            }
        }

        public void Recycle(GridPathElement<TTile, TTerrain, TPosition, TContext> i_Data)
        {
            i_Data.Clear();
            lock (m_SyncLock)
            {
                m_Data.Add(i_Data);
            }
        }

        public void RecycleMultiple(List<GridPathElement<TTile, TTerrain, TPosition, TContext>> i_Data)
        {
            lock (m_SyncLock)
            {
                int count = i_Data.Count;
                for (int i = 0; i < count; ++i)
                {
                    var element = i_Data[i];
                    element.Clear();
                    m_Data.Add(element);
                }
            }
            i_Data.Clear();
        }

        public static readonly GridPathElementPool<TTile, TTerrain, TPosition, TContext> GLOBAL = new GridPathElementPool<TTile, TTerrain, TPosition, TContext>(50);
    }
}
