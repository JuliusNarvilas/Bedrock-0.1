using AssetBundles;
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

    [Serializable]
    public class AssetReferenceInfo
    {
        public string GUID;
        public AssetReferenceType Type;
        public string FilePath;
        public string BundleName;
        public string Varient;
    }

    public class AssetDataReference
    {
        /// <summary>
        /// resources: filepath
        /// bundles: bundleNames
        /// </summary>
        public readonly string Src;
        /// <summary>
        /// resources: filename
        /// bundles: filename
        /// </summary>
        public readonly string Name;
        public readonly AssetReferenceType Type;

        private AssetReferenceLoadHandle m_InitialLoad;
        private int m_RefCount = 0;
        private List<UnityEngine.Object> m_Cache = new List<UnityEngine.Object>();

        public T LoadAsync<T>(string i_SubName = null) where T : UnityEngine.Object
        {
            return LoadAsync(typeof(T), i_SubName) as T;
        }

        public AssetReferenceLoadHandle LoadAsync(System.Type i_Type, string i_SubName = null)
        {
            ScriptableObject test;
            switch(Type)
            {
                case AssetReferenceType.AssetBundle:
                    //load if not loaded before
                    if (m_Cache.Count <= 0)
                    {
                        AssetBundleManager.LoadAssetAsync();
                        string err;
                        var bundle = AssetBundleManager.GetLoadedAssetBundle(Src, out err);
                        if(!string.IsNullOrEmpty(i_SubName))
                        {
                            var newAssets = bundle.m_AssetBundle.LoadAssetWithSubAssets(Name, i_Type);
                            int count = newAssets.Count();
                            for(int i = 0; i < count; ++i)
                            {
                                m_Cache.Add(newAssets[i]);
                            }
                        }
                        if (!string.IsNullOrEmpty(i_SubName))
                        {
                            m_Cache.Add(bundle.m_AssetBundle.LoadAsset(Name));
                        }
                    }
                    //load if it's the first sub-asset load request
                    else if (m_Cache.Count == 1 && !string.IsNullOrEmpty(i_SubName))
                    {
                        string err;
                        AssetBundleManager.LoadAssetAsync();
                        var bundle = AssetBundleManager.GetLoadedAssetBundle(Src, out err);
                        var temp = m_Cache[0];
                        m_Cache.Clear();
                        var newAssets = bundle.m_AssetBundle.LoadAssetWithSubAssets(Name, i_Type);
                        int count = newAssets.Count();
                        for (int i = 0; i < count; ++i)
                        {
                            m_Cache.Add(newAssets[i]);
                        }
                        m_Cache.Add(temp);
                    }
                    break;
                case AssetReferenceType.Resource:
                    //load if not loaded before
                    if (m_Cache.Count <= 0)
                    {
                        m_Cache.AddRange(Resources.LoadAll(Src, i_Type));
                        if (!string.IsNullOrEmpty(i_SubName))
                        {
                            m_Cache.Add(Resources.Load(Src));
                        }
                    }
                    //load if it's the first sub-asset load request
                    else if (m_Cache.Count == 1 && !string.IsNullOrEmpty(i_SubName))
                    {
                        var parentAsset = m_Cache[0];
                        m_Cache.Clear();
                        m_Cache.AddRange(Resources.LoadAll(Src, i_Type));
                        m_Cache.Add(parentAsset);
                    }
                    break;
            }

            
            if (string.IsNullOrEmpty(i_SubName))
            {
                ++m_RefCount;
                return m_Cache.Last();
            }
            else
            {
                foreach (var cacheItem in m_Cache)
                {
                    if (cacheItem.name == i_SubName)
                    {
                        ++m_RefCount;
                        return cacheItem;
                    }
                }
            }
            return null;
        }

        public void Unload()
        {
            --m_RefCount;
            if (m_RefCount <= 0)
            {
                int count = m_Cache.Count;
                switch(Type)
                {
                    case AssetReferenceType.AssetBundle:
                        string err;
                        var bundle = AssetBundleManager.GetLoadedAssetBundle(Src, out err);
                        for (int i = 0; i < count; ++i)
                        {
                            bundle.m_AssetBundle.Unload
                            Resources.UnloadAsset(m_Cache[i]);
                        }
                        m_Cache.Clear();
                        Resources.UnloadUnusedAssets();
                        break;
                    case AssetReferenceType.Resource:
                        for (int i = 0; i < count; ++i)
                        {
                            Resources.UnloadAsset(m_Cache[i]);
                        }
                        m_Cache.Clear();
                        Resources.UnloadUnusedAssets();
                        break;
                }
                m_RefCount = 0;
            }
        }

    }

    public class AssetReferenceTracker : ScriptableObject, ISerializationCallbackReceiver, IDisposable
    {
        [Serializable]
        private struct AssetDirectoryData
        {
            public string FilePath;
            public string GUID;
        }

        private static readonly string RESOURCES_STR = "Resources";
        public static readonly string ResourcesWithSeperators = Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar;
        private static AssetReferenceTracker s_Instance = null;
        public static AssetReferenceTracker FindInstance()
        {
            return Resources.Load<AssetReferenceTracker>("AssetReferenceTracker");
        }
        public static AssetReferenceTracker Instance
        {
            get
            {
                if (s_Instance != null)
                    return s_Instance;
                s_Instance = FindInstance();
                if (s_Instance != null)
                    return s_Instance;
                s_Instance = CreateInstance<AssetReferenceTracker>();
#if UNITY_EDITOR
                var resDir = new DirectoryInfo(Path.Combine(Application.dataPath, RESOURCES_STR));
                if (!resDir.Exists)
                    AssetDatabase.CreateFolder("Assets", RESOURCES_STR);
                AssetDatabase.CreateAsset(s_Instance, "Assets/Resources/AssetReferenceTracker.asset");
                s_Instance = FindInstance();
#endif
                return s_Instance;
            }
        }

        private AssetReferenceTracker()
        {
            s_Instance = this;
        }

#if UNITY_EDITOR
        [MenuItem("Tools/Rescan AssetReferenceTracker")]
        internal static void TriggerUpdate()
        {
            Instance.Rescan();
        }
#endif

        private Dictionary<string, AssetReferenceInfo> m_GUIDToAssetInfo = new Dictionary<string, AssetReferenceInfo>();
        private Dictionary<string, AssetDataReference> m_AssetPathToAssetData = new Dictionary<string, AssetDataReference>();

        [SerializeField, HideInInspector]
        private List<AssetReferenceInfo> m_AssetInfoList;
        [SerializeField, HideInInspector]
        private List<AssetDirectoryData> m_AssetBundleDirectories = new List<AssetDirectoryData>();


        public bool Remove(string assetGUID)
        {
            return m_GUIDToAssetInfo.Remove(assetGUID);
        }

        public bool Update(string assetGUID)
        {
            var assetInfo = FindAssetInfo(assetGUID);
            if (assetInfo != null)
            {
                switch(assetInfo.Type)
                {
                    case AssetReferenceType.AssetBundle:
                        m_GUIDToAssetInfo[assetGUID] = assetInfo;
                        return true;
                    case AssetReferenceType.AssetBundleDirectory:
                        m_AssetBundleDirectories.Add(new AssetDirectoryData() { GUID = assetGUID, FilePath = assetInfo.FilePath });
                        return true;
                    case AssetReferenceType.Resource:
                        m_GUIDToAssetInfo[assetGUID] = assetInfo;
                        return true;
                }
            }
            return false;
        }

        public AssetReferenceInfo GetInfo(string assetGUID)
        {
            AssetReferenceInfo result;
            m_GUIDToAssetInfo.TryGetValue(assetGUID, out result);
            return result;
        }

        public AssetDataReference GetData(string assetGUID)
        {
            var info = GetInfo(assetGUID);
            if(info != null)
            {
                AssetDataReference result;
                m_AssetPathToAssetData.TryGetValue(info.FilePath, out result);
                return result;
            }
            return null;
        }

        
        private AssetReferenceInfo FindAssetInfo(string assetGUID)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
            var importer = AssetImporter.GetAtPath(assetPath);
            string bundleName = importer.assetBundleName;

            if (!string.IsNullOrEmpty(bundleName))
            {
                var attributes = File.GetAttributes(assetPath);
                AssetReferenceType type = (attributes & FileAttributes.Directory) == FileAttributes.Directory ? AssetReferenceType.AssetBundleDirectory: AssetReferenceType.AssetBundle;

                return new AssetReferenceInfo()
                {
                    GUID = assetGUID,
                    Type = type,
                    FilePath = assetPath,
                    BundleName = bundleName,
                    Varient = importer.assetBundleVariant
                };
            }
            else if (assetPath.IndexOf(ResourcesWithSeperators) >= 0)
            {
                return new AssetReferenceInfo()
                {
                    GUID = assetGUID,
                    Type = AssetReferenceType.Resource,
                    FilePath = assetPath,
                };
            }
            else
            {
                var attributes = File.GetAttributes(assetPath);
                AssetReferenceType type = (attributes & FileAttributes.Directory) == FileAttributes.Directory ? AssetReferenceType.AssetBundleDirectory : AssetReferenceType.AssetBundle;

                foreach (var directoryData in m_AssetBundleDirectories)
                {
                    if (assetPath.IndexOf(directoryData.FilePath) == 0)
                    {
                        var directoryAssetInfo = m_GUIDToAssetInfo[directoryData.GUID];
                        var newAssetVariant = importer.assetBundleVariant;
                        return new AssetReferenceInfo()
                        {
                            GUID = assetGUID,
                            Type = type,
                            FilePath = assetPath,
                            BundleName = directoryAssetInfo.BundleName,
                            Varient = string.IsNullOrEmpty(newAssetVariant) ? directoryAssetInfo.Varient : newAssetVariant
                        };
                    }
                }
            }
            return null;
        }

        public void Rescan(bool withSave = true)
        {
            m_GUIDToAssetInfo.Clear();
            foreach (var assetGuid in AssetDatabase.FindAssets(string.Empty))
            {
                Update(assetGuid);
            }
            if (withSave)
            {
                Save();
            }
        }

        public void Save()
        {
            m_AssetInfoList = m_GUIDToAssetInfo.Values.ToList();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        public void OnBeforeSerialize()
        {
            if(m_AssetInfoList == null)
            {
                Rescan(false);
            }
        }

        public void OnAfterDeserialize()
        {
            m_GUIDToAssetInfo.Clear();
            int count = m_AssetInfoList.Count;
            for (int i = 0; i < count; ++i)
            {
                var info = m_AssetInfoList[i];
                m_GUIDToAssetInfo[info.GUID] = info;
            }
        }

        public void Dispose()
        {
            m_GUIDToAssetInfo.Clear();

        }
    }


#if UNITY_EDITOR

    public class ResourceDBPostprocessor : UnityEditor.AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (AssetReferenceTracker.FindInstance() == null)
                return;

            bool needsToSave = false;
            foreach (var filePath in deletedAssets)
            {
                string guid = AssetDatabase.AssetPathToGUID(filePath);
                if(!string.IsNullOrEmpty(guid) && AssetReferenceTracker.Instance.Remove(guid))
                {
                    needsToSave = true;
                }
            }

            foreach (var assetPath in movedAssets)
            {
                needsToSave = needsToSave || AssetReferenceTracker.Instance.Update(AssetDatabase.AssetPathToGUID(assetPath));
            }
            if (needsToSave)
            {
                AssetReferenceTracker.Instance.Save();
            }
        }
    }
#endif
}
