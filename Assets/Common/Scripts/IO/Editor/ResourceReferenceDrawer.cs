using UnityEditor;
using UnityEngine;

namespace Common.IO
{
    [CustomPropertyDrawer(typeof(ResourceReference))]
    public class ResourceReferenceDrawer : PropertyDrawer
    {
        private Rect m_ContentArea;

        // Draw the property inside the given rect
        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            m_ContentArea = pos;
            EditorGUI.BeginProperty(m_ContentArea, label, prop);
            m_ContentArea.height = EditorGUIExtensions.DEFAULT_GUI_HEIGHT;
            ResourcesDBItem tempResourceItem = null;
            SerializedProperty assetProp = prop.FindPropertyRelative("FilePath");
            if (assetProp != null)
            {
                string assetPath = assetProp.stringValue;
                Object asset = null;
                if (!string.IsNullOrEmpty(assetPath))
                {
                    System.Type type = typeof(Object);
                    string subName = prop.FindPropertyRelative("SubName").stringValue;
                    if (!string.IsNullOrEmpty(subName))
                    {
                        string typeName = prop.FindPropertyRelative("SubType").stringValue;
                        type = System.Type.GetType(typeName);
                    }
                    tempResourceItem = ResourcesDB.GetByResourcesPath(assetPath);
                    asset = tempResourceItem.Load(type, subName);
                }
                asset = EditorGUI.ObjectField(m_ContentArea, "Asset", asset, typeof(Object), false);
                string newAssetFilePath = string.Empty;
                if (asset != null)
                {
                    string foundAssetPath = AssetDatabase.GetAssetPath(asset);
                    string foundAssetName = System.IO.Path.GetFileNameWithoutExtension(foundAssetPath);
                    newAssetFilePath = System.IO.Path.ChangeExtension(foundAssetPath, null);
                    ResourcesDBItem info = ResourcesDB.GetByPath(newAssetFilePath);
                    //noverting path to resource path
                    newAssetFilePath = (info != null) ? info.ResourcesPath : string.Empty;

                    if (asset.name != foundAssetName)
                    {
                        prop.FindPropertyRelative("SubName").stringValue = asset.name;
                        prop.FindPropertyRelative("SubType").stringValue = asset.GetType().AssemblyQualifiedName;
                    }
                    else
                    {
                        prop.FindPropertyRelative("SubName").stringValue = string.Empty;
                        prop.FindPropertyRelative("SubType").stringValue = string.Empty;
                    }
                }
                assetProp.stringValue = newAssetFilePath;
                if (tempResourceItem != null)
                {
                    tempResourceItem.Unload();
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return EditorGUIExtensions.DEFAULT_GUI_LINE_HEIGHT;
        }
    }
}
