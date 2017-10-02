
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Common.IO
{

    public interface UpdateRunnerTask
    {
        bool UpdateAndFinish();
    }

    //[InitializeOnLoad]
    [ExecuteInEditMode]
    public class UpdateRunner : SingletonMonoBehaviour<UpdateRunner>
    {

        private object m_lockObj = new object();
        private bool m_Executing = false;
        private List<UpdateRunnerTask> m_Tasks = new List<UpdateRunnerTask>();

        //Force UpdateRunner to update in edit mode as it does in play mode
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            Instance.Awake();
        }
        private new void Awake()
        {
            m_Executing = false;
            Log.DebugLog("UpdateRunner Started.");
            base.Awake();
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                EditorApplication.update -= Instance.EditorUpdate;
            }
            else
            {
                EditorApplication.update += Instance.EditorUpdate;
            }
#endif
        }

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

#if UNITY_EDITOR
        private void Update()
        {
            if(Application.isPlaying)
            {
                EditorUpdate();
            }
        }

        private void EditorUpdate()
        {
#else
        private void Update()
        {
#endif
            if (m_Tasks.Count > 0)
            {
                lock (m_lockObj)
                {

                    //prevent nested editor updates
                    if (!m_Executing)
                    {
                        m_Executing = true;
                        int count = m_Tasks.Count;
                        for (int i = count - 1; i >= 0; --i)
                        {
                            bool result = m_Tasks[i].UpdateAndFinish();
                            if (result)
                            {
                                m_Tasks.RemoveAt(i);
                            }
                        }
                        m_Executing = false;
                    }
                }
            }
        }
    }
}
