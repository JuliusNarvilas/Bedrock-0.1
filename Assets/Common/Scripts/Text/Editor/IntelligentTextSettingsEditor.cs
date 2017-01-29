using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEditor;

namespace Common.Text
{
    [CustomEditor(typeof(IntelligentTextSettings))]
    public class IntelligentTextSettingsEditor : Editor
    {
        IntelligentTextSettings m_Target;
        void OnEnable()
        {
            m_Target = (IntelligentTextSettings)target;
        }

        public override void OnInspectorGUI()
        {
            //GUI.enabled = false;
            DrawDefaultInspector();
            //GUI.enabled = true;
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Reload"))
            {
                m_Target.Reload();
                m_Target.RefreshText();
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(m_Target);
                AssetDatabase.SaveAssets();
            }
            GUILayout.EndHorizontal();
        }
    }
}
