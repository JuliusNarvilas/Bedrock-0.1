using UnityEngine;

namespace Common.Text
{
    public class IntelligentTextUpdater : IntelligentText
    {
        public enum UpdateType
        {
            Update,
            Rebuild,
            Refresh
        }
        
        private float m_TimeAccumulator = 0;

        public float UpdateTimer = 0;
        public UpdateType Type = UpdateType.Update;
        public bool ScaledTime = true;

        private void Update()
        {
            m_TimeAccumulator += ScaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
            if(m_TimeAccumulator >= UpdateTimer)
            {
                m_TimeAccumulator -= UpdateTimer;
                switch(Type)
                {
                    case UpdateType.Update:
                        UpdateText();
                        break;
                    case UpdateType.Rebuild:
                        RebuildText();
                        break;
                    case UpdateType.Refresh:
                        Refresh();
                        break;
                }
            }
        }
    }
}
