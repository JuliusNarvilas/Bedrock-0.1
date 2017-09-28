using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Common.IO
{
    public enum AssetReferenceType
    {
        AssetBundle,
        AssetBundleDirectory,
        Resource
    }
    /*
    public enum AssetReferenceDataType
    {
        Bytes,
        AssetBundle,
        AudioClip,
        MovieTexture,
        Text,
        Texture2D,
        Texture2DNonReadable
    }
        public static readonly string ResourcesWithSeperators = System.IO.Path.DirectorySeparatorChar + "Resources" + System.IO.Path.DirectorySeparatorChar;
        public static readonly string StreamingAssetsWithSeperators = System.IO.Path.DirectorySeparatorChar + "StreamingAssets" + System.IO.Path.DirectorySeparatorChar;
    */

    public class AssetRecord
    {

    }

    [Serializable]
    public class AssetReference : IDisposable
    {
        [SerializeField]
        private string m_GUID;
        /// <summary>
        /// For storing nested asset type information.
        /// </summary>
        [SerializeField]
        private string m_TypeStr;
        /// <summary>
        /// For nested assets like Sprites.
        /// </summary>
        [SerializeField]
        private string m_SubName;

        private AssetDataReference m_DataReference;

        private Type m_Type;
        public Type GetAssetType()
        {
            if (m_Type == null)
            {
                m_Type = System.Type.GetType(m_TypeStr);
            }
            return m_Type;
        }

        private AssetReference(string i_GUID, string i_TypeStr, string i_SubName)
        {
            m_GUID = i_GUID;
            m_TypeStr = i_TypeStr;
            m_SubName = i_SubName;
        }

        public string GetPath()
        {
            var info = AssetReferenceTracker.Instance.GetInfo(m_GUID);
            if(info != null)
            {
                return info.FilePath;
            }
            return null;
        }

        public AssetReferenceType GetAssetRefType()
        {
            var info = AssetReferenceTracker.Instance.GetInfo(m_GUID);
            if (info != null)
            {
                return info.Type;
            }
            return default(AssetReferenceType);
        }

        public T Get<T>() where T : UnityEngine.Object
        {
            var dataRef = AssetReferenceTracker.Instance.GetData(m_GUID);
            if (dataRef != null)
            {
                m_DataReference = dataRef;
                if (!string.IsNullOrEmpty(m_SubName))
                {
                    return m_DataReference.Load(GetType(), m_SubName) as T;
                }
                return m_DataReference.Load<T>();
            }
            return null;
        }

        public AssetReference Clone()
        {
            return new AssetReference(m_GUID, m_TypeStr, m_SubName);
        }

        public void Dispose()
        {
            if(m_DataReference != null)
            {
                m_DataReference.Unload();
                m_DataReference = null;
            }
        }

        /*
        public void Get()
        {
            AssetReferenceType referenceType = AssetReferenceType.AssetBundle;
            switch(referenceType)
            {
                case AssetReferenceType.AssetBundle:
                    break;
                case AssetReferenceType.Resource:
                    break;
                case AssetReferenceType.StreamingAssetLocal:
                    break;
                case AssetReferenceType.StreamingAssetUrl:
                    break;
            }
            WWW data = new WWW(Application.streamingAssetsPath + "/" + "");
            
            //data.Get
        }
        */

        /*
    public static UnityEngine.Object GUIDToObject(string i_GUID)
    {
        string assetPath = AssetDatabase.GUIDToAssetPath(i_GUID);
        if (!string.IsNullOrEmpty(assetPath))
        {
            return AssetDatabase.LoadMainAssetAtPath(assetPath);
        }
        return null;
    }
    */


        /*
        public static AssetReferenceType FindObjectAssetType(UnityEngine.Object i_UnityObject)
        {
            AssetDatabase.AssetPathToGUID();
            i_UnityObject.
            string assetPath = AssetDatabase.GetAssetPath(i_UnityObject);
            string bundleName = AssetImporter.GetAtPath(assetPath).assetBundleName;

            if (!string.IsNullOrEmpty(bundleName))
            {
                return AssetReferenceType.AssetBundle;
            }
            else if (assetPath.IndexOf(ResourcesWithSeperators) >= 0)
            {
                return AssetReferenceType.Resource;
            }
            else if (assetPath.IndexOf(StreamingAssetsWithSeperators) >= 0)
            {
                return AssetReferenceType.StreamingAssetLocal;
            }
        }



        IEnumerator Example()
        {
            if (filePath.Contains("://"))
            {
                WWW www = new WWW(filePath);
                yield return www;
                result = www.text;
            }
            else
                result = System.IO.File.ReadAllText(filePath);
        }

        private void OnValidate()
        {
            Debug.Log(Obj.name);
            Debug.Log(AssetDatabase.GetAssetPath(Obj));
        }
        */
    }
}
