using UnityEngine;
using UnityEditor;

namespace Common.IO
{

    [CustomEditor(typeof(ResourcesDB))]
    public class ResourceDBEditor : Editor
    {
        ResourcesDB m_Target;
        void OnEnable()
        {
            m_Target = (ResourcesDB)target;
        }
        public override void OnInspectorGUI()
        {

            GUI.enabled = false;
            DrawDefaultInspector();
            GUI.enabled = true;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Update Now"))
            {
                m_Target.UpdateDB(true);
            }
            m_Target.UpdateAutomatically = GUILayout.Toggle(m_Target.UpdateAutomatically, "AutoUpdate", "Button");
            if (GUI.changed)
            {
                EditorUtility.SetDirty(m_Target);
                AssetDatabase.SaveAssets();
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Folders:", m_Target.FolderCount.ToString());
            EditorGUILayout.LabelField("Files:", m_Target.FileCount.ToString());
        }
    }
}