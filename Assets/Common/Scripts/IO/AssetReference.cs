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
        private string Guid;
        /// <summary>
        /// For nested assets like Sprites.
        /// </summary>
        [SerializeField]
        private string SubName;
        /// <summary>
        /// For storing nested asset type information.
        /// Is mostly for editor scripts
        /// </summary>
        [SerializeField]
        private string TypeStr;

        private AssetDataReference m_DataReference;

        private Type m_Type;
        public Type GetAssetType()
        {
            if (m_Type == null)
            {
                m_Type = System.Type.GetType(TypeStr);
            }
            return m_Type;
        }

        private AssetReference(string i_GUID, string i_TypeStr, string i_SubName)
        {
            Guid = i_GUID;
            TypeStr = i_TypeStr;
            SubName = i_SubName;
        }

        public string GetPath()
        {
            var info = AssetReferenceTracker.Instance.GetAssetInfo(Guid);
            if(info != null)
            {
                return info.FilePath;
            }
            return null;
        }

        public AssetReferenceType GetAssetRefType()
        {
            var info = AssetReferenceTracker.Instance.GetAssetInfo(Guid);
            if (info != null)
            {
                return info.ReferenceType;
            }
            return default(AssetReferenceType);
        }

        public AssetReferenceLoadHandle GetAsync<T>() where T : UnityEngine.Object
        {
            var dataRef = AssetReferenceTracker.Instance.GetAssetData(Guid);
            if (dataRef != null)
            {
                return dataRef.LoadAsync(typeof(T), SubName);
            }
            return null;
        }

        public AssetReference Clone()
        {
            return new AssetReference(Guid, TypeStr, SubName);
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
