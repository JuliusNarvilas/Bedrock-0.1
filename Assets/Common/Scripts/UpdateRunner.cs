using AssetBundles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Common.IO
{

    public interface UpdateRunnerTask
    {
        bool UpdateAndFinish();
    }

    public class UpdateRunner : SingletonMonoBehaviour<UpdateRunner>
    {
        private object m_lockObj = new object();
        private List<UpdateRunnerTask> m_Tasks = new List<UpdateRunnerTask>();

        public void AddTask(UpdateRunnerTask i_Task)
        {
            lock (m_lockObj)
            {
                m_Tasks.Add(i_Task);
            }
        }

        public void AddTaskUnsafe(UpdateRunnerTask i_Task)
        {
            m_Tasks.Add(i_Task);
        }

        private void Update()
        {
            lock (m_lockObj)
            {
                int count = m_Tasks.Count;
                for (int i = count - 1; i >= 0; --i)
                {
                    if (m_Tasks[i].UpdateAndFinish())
                    {
                        m_Tasks.RemoveAt(i);
                    }
                }
            }
        }
    }
}
