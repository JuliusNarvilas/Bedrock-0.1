using UnityEngine;

namespace Common.Text
{
    public class IntelligentTextUpdater : IntelligentText
    {
        public enum ETextUpdateType
        {
            /// <summary>
            /// Rebuilds the mesh.
            /// </summary>
            Update,
            /// <summary>
            /// Rebuilds the text.
            /// </summary>
            Rebuild,
            /// <summary>
            /// Reconfigures the parser.
            /// </summary>
            Refresh
        }
        
        private float m_TimeAccumulator = 0;

        public float UpdateTimer = 0;
        public ETextUpdateType Type = ETextUpdateType.Update;
        public bool ScaledTime = true;

        private void Update()
        {
            m_TimeAccumulator += ScaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
            if(m_TimeAccumulator >= UpdateTimer)
            {
                m_TimeAccumulator -= UpdateTimer;
                switch(Type)
                {
                    case ETextUpdateType.Update:
                        UpdateText();
                        break;
                    case ETextUpdateType.Rebuild:
                        RebuildText();
                        break;
                    case ETextUpdateType.Refresh:
                        Refresh();
                        break;
                }
            }
        }
    }
}
