using AssetBundles;
using Common.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public readonly AssetReferenceType ReferenceType;

        private AssetReferenceLoadHandle m_InitialLoad;
        private int m_RefCount = 0;
        private List<UnityEngine.Object> m_Cache = new List<UnityEngine.Object>();

        public T LoadAsync<T>(string i_SubName = null) where T : UnityEngine.Object
        {
            return LoadAsync(typeof(T), i_SubName) as T;
        }

        public AssetReferenceLoadHandle LoadAsync(System.Type i_Type, string i_SubName = null)
        {
            //load if not loaded before
            if (m_Cache.Count <= 0)
            {
                if (m_InitialLoad == null)
                {
                    switch (ReferenceType)
                    {
                        case AssetReferenceType.AssetBundle:
                            {
                                var operation = AssetBundleManager.LoadAssetAsync(Src, Name, i_Type);
                                var operationHandle = new AssetReferenceAssetBundleLoadHandle(operation);
                                operationHandle.LoadCallback = (UnityEngine.Object newAsset) =>
                                {
                                    m_Cache.Add(newAsset);
                                    m_InitialLoad = null;
                                };
                                m_InitialLoad = operationHandle;
                            }
                            break;
                        case AssetReferenceType.Resource:
                            {
                                var operation = Resources.LoadAsync(Src);
                                var operationHandle = new AssetReferenceResourceLoadHandle(operation);
                                operationHandle.LoadCallback = (UnityEngine.Object newAsset) =>
                                {
                                    m_Cache.Add(newAsset);
                                    m_InitialLoad = null;
                                };
                                m_InitialLoad = operationHandle;
                            }
                            break;
                    }
                }

                //only load nested assets after the parent one for correct
                //bundle loading and reference count tracking done by AssetBundleManager
                if (!string.IsNullOrEmpty(i_SubName) && m_InitialLoad != null)
                {
                    //all subassets will be already added by AssetReferenceDelayedLoadHandle
                    return new AssetReferenceDelayedLoadHandle(m_InitialLoad, this, i_Type, i_SubName);
                }

                Log.DebugAssert(m_InitialLoad != null, "AssetDataReference: invalid asset loading requested ({0} ; {1} ; {2}).", Src, Name, ReferenceType.ToString());
                return m_InitialLoad;
            }
            

            //requesting existing parrent asset
            if (string.IsNullOrEmpty(i_SubName))
            {
                return new AssetReferenceNoLoadHandle(m_Cache[0]);
            }
            //requesting child asset
            else
            {
                //load child assets
                if (m_Cache.Count == 1)
                {
                    UnityEngine.Object[] newAssets = null;
                    switch (ReferenceType)
                    {
                        case AssetReferenceType.AssetBundle:
                            {
                                string error;
                                var assetBundle = AssetBundleManager.GetLoadedAssetBundle(Src, out error);
                                newAssets = assetBundle.m_AssetBundle.LoadAssetWithSubAssets(Name, i_Type);
                            }
                            break;
                        case AssetReferenceType.Resource:
                            newAssets = Resources.LoadAll(Src, i_Type);
                            break;
                    }
                    if (newAssets != null)
                    {
                        int count = newAssets.Length;
                        for (int i = 0; i < count; ++i)
                        {
                            m_Cache.Add(newAssets[i]);
                        }
                    }
                }
                //load from cache
                int cacheItemCount = m_Cache.Count;
                for (int i = 0; i < cacheItemCount; ++i)
                {
                    if (m_Cache[i].name == i_SubName)
                    {
                        return new AssetReferenceNoLoadHandle(m_Cache[i]);
                    }
                }
            }

            Log.DebugLogError("AssetDataReference: invalid asset loading requested ({0} ; {1} ; {2}).", Src, Name, ReferenceType.ToString());
            return null;
        }

        public void Unload()
        {
            if (m_RefCount > 0)
            {
                --m_RefCount;
                if (m_RefCount == 0)
                {
                    switch (ReferenceType)
                    {
                        case AssetReferenceType.AssetBundle:
                            {
                                AssetBundleManager.UnloadAssetBundle(Src);
                                m_Cache.Clear();
                            }
                            break;
                        case AssetReferenceType.Resource:
                            {
                                int count = m_Cache.Count;
                                for (int i = 0; i < count; ++i)
                                {
                                    Resources.UnloadAsset(m_Cache[i]);
                                }
                                m_Cache.Clear();
                                //Resources.UnloadUnusedAssets();
                            }
                            break;
                    }
                }
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
