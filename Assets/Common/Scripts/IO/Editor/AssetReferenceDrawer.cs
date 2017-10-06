 using Common.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Common.IO
{
    [CustomPropertyDrawer(typeof(AssetReference))]
    public class AssetReferenceDrawer : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            File.AppendAllText("C:\\Users\\juliu\\Desktop\\Test.txt", "START\n");

            var contentArea = pos;
            EditorGUI.BeginProperty(contentArea, label, prop);
            contentArea.height = EditorHelpers.DEFAULT_GUI_HEIGHT;
            
            bool assetIsLoading = false;
            SerializedProperty assetGuidProp = prop.FindPropertyRelative("Guid");
            if (assetGuidProp != null)
            {
                string assetGuid = assetGuidProp.stringValue;
                UnityEngine.Object asset = null;
                if (!string.IsNullOrEmpty(assetGuid))
                {
                    File.AppendAllText("C:\\Users\\juliu\\Desktop\\Test.txt", "Property old guid found\n");

                    System.Type type = typeof(UnityEngine.Object);
                    string subName = prop.FindPropertyRelative("SubName").stringValue;
                    if (!string.IsNullOrEmpty(subName))
                    {
                        string typeName = prop.FindPropertyRelative("TypeStr").stringValue;
                        type = System.Type.GetType(typeName);
                        File.AppendAllText("C:\\Users\\juliu\\Desktop\\Test.txt", "Property old type found\n");
                    }

                    var dataReference = AssetReferenceTracker.Instance.GetAssetData(assetGuid);
                    //if valid tracked asset
                    if (dataReference != null)
                    {
                        File.AppendAllText("C:\\Users\\juliu\\Desktop\\Test.txt", "Property old data reference found\n");
                        
                        AssetReferenceLoadHandle loadHandle = dataReference.LoadAsync(type, subName);
                        if (loadHandle.IsDone())
                        {
                            asset = loadHandle.GetAsset<UnityEngine.Object>();
                        }
                        else
                        {
                            assetIsLoading = true;
                        }
                    }
                }
                File.AppendAllText("C:\\Users\\juliu\\Desktop\\Test.txt", "Property old data done\n");

                if (assetIsLoading)
                {
                    asset = EditorGUI.ObjectField(contentArea, "Asset", asset, typeof(UnityEngine.Object), false);

                    var labelPos = contentArea;
                    labelPos.width = EditorGUIUtility.labelWidth;
                    var contentPos = contentArea;
                    contentPos.x += labelPos.width;
                    contentPos.width = contentArea.width - labelPos.width;

                    EditorGUI.LabelField(labelPos, "Warning");
                    EditorGUI.SelectableLabel(contentPos, "Asset is loading, refresh to see changes.");
                }
                else
                {
                    asset = EditorGUI.ObjectField(contentArea, "Asset", asset, typeof(UnityEngine.Object), false);

                    if (asset != null)
                    {
                        File.AppendAllText("C:\\Users\\juliu\\Desktop\\Test.txt", "Property new asset found\n");

                        contentArea.y += EditorGUIUtility.singleLineHeight;
                        var labelPos = contentArea;
                        labelPos.width = EditorGUIUtility.labelWidth;
                        var contentPos = contentArea;
                        contentPos.x += labelPos.width;
                        contentPos.width = contentArea.width - labelPos.width;

                        string foundAssetPath = AssetDatabase.GetAssetPath(asset);
                        string newAssetGuid = AssetDatabase.AssetPathToGUID(foundAssetPath);
                        var info = AssetReferenceTracker.Instance.GetAssetInfo(newAssetGuid);
                        //if valid tracked asset
                        if (info != null)
                        {
                            File.AppendAllText("C:\\Users\\juliu\\Desktop\\Test.txt", "Property new asset info found\n");

                            EditorGUI.LabelField(labelPos, "Reference Type");
                            EditorGUI.SelectableLabel(contentPos, info.ReferenceType.ToString());
                            
                            assetGuidProp.stringValue = newAssetGuid;

                            string foundAssetName = System.IO.Path.GetFileNameWithoutExtension(foundAssetPath);
                            //if asset file name doesn't match asset name, it's a nested asset and extra data is required
                            if (asset.name != foundAssetName)
                            {
                                prop.FindPropertyRelative("SubName").stringValue = asset.name;
                                prop.FindPropertyRelative("TypeStr").stringValue = asset.GetType().AssemblyQualifiedName;

                            }
                            else
                            {
                                prop.FindPropertyRelative("SubName").stringValue = null;
                                prop.FindPropertyRelative("TypeStr").stringValue = null;
                            }
                        }
                        else
                        {
                            prop.FindPropertyRelative("SubName").stringValue = null;
                            prop.FindPropertyRelative("TypeStr").stringValue = null;
                            assetGuidProp.stringValue = null;
                        }
                    }
                    else
                    {
                        prop.FindPropertyRelative("SubName").stringValue = null;
                        prop.FindPropertyRelative("TypeStr").stringValue = null;
                        assetGuidProp.stringValue = null;
                    }
                }
            }
            File.AppendAllText("C:\\Users\\juliu\\Desktop\\Test.txt", "Property END\n");

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2;
        }
    }
}
