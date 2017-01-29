using System;
using System.Threading;

namespace Common
{

    public struct MutexLock : IDisposable
    {
        private Mutex m_Mutex;

        public MutexLock(Mutex i_Mutex)
        {
            Log.DebugAssert(i_Mutex != null, "Invalid null property.");
            m_Mutex = i_Mutex;
            m_Mutex.WaitOne();
        }

        public void Dispose()
        {
            if (m_Mutex != null)
            {
                m_Mutex.ReleaseMutex();
                m_Mutex = null;
            }
        }
    }
}
