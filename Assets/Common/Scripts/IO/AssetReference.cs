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

        public AssetReferenceLoadHandle GetAsync<T>() where T : UnityEngine.Object
        {
            var dataRef = AssetReferenceTracker.Instance.GetData(m_GUID);
            if (dataRef != null)
            {
                return dataRef.LoadAsync(typeof(T), m_SubName);
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
        
    }
}
