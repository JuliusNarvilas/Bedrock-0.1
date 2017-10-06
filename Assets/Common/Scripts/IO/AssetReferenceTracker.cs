using AssetBundles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Common.IO
{

    [Serializable]
    public class AssetReferenceInfo
    {
        public string Guid;
        public AssetReferenceType ReferenceType;
        public string FilePath;
        public string BundleName;
        public string Varient;
    }

    /// <summary>
    /// Asset data resource tracking reference for when assets are actually requested.
    /// The data reference is created per asset file and will manage available subassets.
    /// </summary>
    public class AssetDataReference
    {
        /// <summary>
        /// Main asset loading data.
        /// When Resource asset is use, this stores the Resource file path.
        /// When AssetBundle asset is used, this stores the asset bundle name.
        /// </summary>
        public string Src;
        /// <summary>
        /// resources: filename
        /// bundles: filename
        /// </summary>
        public string Name;
        /// <summary>
        /// The type of asset reference.
        /// </summary>
        public AssetReferenceType ReferenceType;

#if UNITY_EDITOR
        public string AssetGuid;
#endif

        /// <summary>
        /// This stores the main referenced asset load handle that will need to finish
        /// before child assets (like sprites in a Texture asset) can be loaded.
        /// Just my code design restriction rather than a Unity requirement.
        /// </summary>
        private AssetReferenceLoadHandle m_InitialLoad;
        private int m_RefCount = 0;
        private List<UnityEngine.Object> m_Cache = new List<UnityEngine.Object>();

        public T LoadAsync<T>(string i_SubName = null) where T : UnityEngine.Object
        {
            return LoadAsync(typeof(T), i_SubName) as T;
        }
        
        public AssetReferenceLoadHandle LoadAsync(System.Type i_Type, string i_SubName = null)
        {

            //initial load
            if (m_Cache.Count <= 0)
            {
                AssetReferenceLoadHandle result = null;
                if (m_InitialLoad == null)
                {
                    switch (ReferenceType)
                    {
                        case AssetReferenceType.AssetBundle:
                            {
                                var operation = AssetBundleManager.LoadAssetAsync(Src, Name, i_Type);
                                m_InitialLoad = new AssetReferenceAssetBundleLoadHandle(operation, (UnityEngine.Object newAsset) =>
                                {
                                    m_Cache.Add(newAsset);
                                    m_InitialLoad = null;
                                });
                                result = m_InitialLoad;
                            }
                            break;
                        case AssetReferenceType.Resource:
                            {
                                //async loading doesn't work in editor mode
                                if (Application.isPlaying)
                                {
                                    var operation = Resources.LoadAsync(Src);
                                    m_InitialLoad = new AssetReferenceResourceLoadHandle(operation, (UnityEngine.Object newAsset) =>
                                    {
                                        m_Cache.Add(newAsset);
                                        m_InitialLoad = null;
                                    });
                                    result = m_InitialLoad;
                                }
                                else
                                {
                                    var newAsset = Resources.Load(Src);
                                    result = new AssetReferenceNoLoadHandle(newAsset);
                                    m_Cache.Add(newAsset);
                                }
                            }
                            break;
                    }
                }
                else
                {
                    result = m_InitialLoad;
                }
                
                //only load nested assets after the parent one. Done for correct
                //bundle loading and reference count tracking done by AssetBundleManager
                if (!string.IsNullOrEmpty(i_SubName))
                {
                    //all subassets will be already added by AssetReferenceDelayedLoadHandle, so using the callback is not required
                    return new AssetReferenceDelayedLoadHandle(m_InitialLoad, this, i_Type, i_SubName);
                }
                else
                {
                    return result;
                }
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
#if UNITY_EDITOR
                                if(AssetBundleManager.SimulateAssetBundleInEditor)
                                {
                                    var assetPath = AssetDatabase.GUIDToAssetPath(AssetGuid);
                                    newAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath).Where(o => i_Type.IsAssignableFrom(o.GetType())).ToArray();
                                }
                                else
#endif
                                {
                                    string error;
                                    var assetBundle = AssetBundleManager.GetLoadedAssetBundle(Src, out error);
                                    newAssets = assetBundle.m_AssetBundle.LoadAssetWithSubAssets(Name, i_Type);
                                }
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
                //0 index is parent asset, so the subasset test can start at 1
                for (int i = 1; i < cacheItemCount; ++i)
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









    /// <summary>
    /// Manager class for <see cref="AssetReference"/> information.
    /// </summary>
    public class AssetReferenceTracker : ScriptableObject, ISerializationCallbackReceiver
    {
        private static readonly string RESOURCES_STR = "Resources";
        public static readonly string ResourcesWithSeperators = "/Resources/";
        public static readonly string AssetReferenceTracherFilePath = "Assets/Resources/AssetReferenceTracker.asset";
        private static AssetReferenceTracker s_Instance = null;
        public static AssetReferenceTracker FindInstance()
        {
            return Resources.Load<AssetReferenceTracker>("AssetReferenceTracker");
        }
        
        /// <summary>
        /// Returns or creates an instance of this manager.
        /// </summary>
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
                AssetDatabase.CreateAsset(s_Instance, AssetReferenceTracherFilePath);
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
#endif
        internal static void TriggerUpdate()
        {
            if (s_Instance != null)
            {
                Instance.Rescan();
            }
            else
            {
                //call to Instance property will instantiate and rescan assets during serialization on manager data.
                Instance.ToString();
            }
        }

        private Dictionary<string, AssetReferenceInfo> m_GuidToAssetInfo = new Dictionary<string, AssetReferenceInfo>();
        private Dictionary<string, AssetReferenceInfo> m_PathToAssetBundleDirectory = new Dictionary<string, AssetReferenceInfo>();
        /// <summary>
        /// Asset data cache that has been requested.
        /// </summary>
        private Dictionary<string, AssetDataReference> m_AssetPathToAssetData = new Dictionary<string, AssetDataReference>();

        [SerializeField, HideInInspector]
        private List<AssetReferenceInfo> m_AssetInfoList;
        [SerializeField, HideInInspector]
        private List<AssetReferenceInfo> m_AssetBundleDirectories = new List<AssetReferenceInfo>();


        public bool RemoveAsset(string assetGuid)
        {
            return m_GuidToAssetInfo.Remove(assetGuid);
        }
        public bool RemoveDirectory(string path)
        {
            return m_PathToAssetBundleDirectory.Remove(path);
        }

        /// <summary>
        /// New asset data is created mapped to the Guid (old data is lost)
        /// </summary>
        /// <param name="assetGuid">Guid of the asset file.</param>
        /// <returns>True if manager state changed and false otherwise.</returns>
        public bool UpdateAsset(string assetGuid)
        {
#if UNITY_EDITOR
            var assetInfo = CreateAssetInfo(assetGuid);

            if (assetInfo != null)
            {
                switch(assetInfo.ReferenceType)
                {
                    case AssetReferenceType.AssetBundle:
                    case AssetReferenceType.Resource:
                        m_GuidToAssetInfo[assetGuid] = assetInfo;
                        return true;
                    case AssetReferenceType.AssetBundleDirectory:
                        m_PathToAssetBundleDirectory[assetInfo.FilePath] = assetInfo;
                        return true;
                }
            }

            bool wasRemoved = m_GuidToAssetInfo.Remove(assetGuid);
            if(!wasRemoved)
            {
                AssetDatabase.GUIDToAssetPath(assetGuid);
                wasRemoved = m_PathToAssetBundleDirectory.Remove(assetGuid);
            }
            return wasRemoved;
#else
            throw new NotSupportedException();
#endif
        }

        public AssetReferenceInfo GetAssetInfo(string assetGuid)
        {
            AssetReferenceInfo result;
            m_GuidToAssetInfo.TryGetValue(assetGuid, out result);
            return result;
        }

        public AssetDataReference GetAssetData(string assetGuid)
        {
            var info = GetAssetInfo(assetGuid);
            if(info != null)
            {
                AssetDataReference result;
                if (m_AssetPathToAssetData.TryGetValue(info.FilePath, out result))
                {
                    return result;
                }

                AssetDataReference dataReference = null;
                string name = Path.GetFileNameWithoutExtension(info.FilePath);
                switch(info.ReferenceType)
                {
                    case AssetReferenceType.AssetBundle:
                        dataReference = new AssetDataReference()
                        {
                            Name = name,
                            ReferenceType = info.ReferenceType,
                            Src = info.BundleName
#if UNITY_EDITOR
                            ,AssetGuid = assetGuid
#endif
                        };
                        break;
                    case AssetReferenceType.Resource:
                        string resourcesDir = "Resources/";
                        string resourcePath = Path.ChangeExtension(info.FilePath, null);
                        int resourceStrStartIndex = resourcePath.IndexOf(resourcesDir);
                        dataReference = new AssetDataReference()
                        {
                            Name = name,
                            ReferenceType = info.ReferenceType,
                            Src = resourcePath.Substring(resourceStrStartIndex + resourcesDir.Length)
#if UNITY_EDITOR
                            ,AssetGuid = assetGuid
#endif
                        };
                        break;
                    case AssetReferenceType.AssetBundleDirectory:
                        Log.DebugLogError("AssetReferenceTracker: GetData() called with unsupported asset GUID!");
                        break;
                }
                m_AssetPathToAssetData[info.FilePath] = dataReference;
                return dataReference;
            }
            return null;
        }

        /// <summary>
        /// Recursive call to find asset bundle directory information for a given asset file path.
        /// </summary>
        /// <param name="filePath">Asset filepath.</param>
        /// <returns>Indicated file's asset bundle directory information or null if no such directory exists.</returns>
        private AssetReferenceInfo FindAssetBundleDirectory(string filePath)
        {
            if(filePath == null)
            {
                return null;
            }
            int lastSeperatorIndex = filePath.LastIndexOf("/");
            if (lastSeperatorIndex <= 0)
                return null;

            filePath = filePath.Substring(0, lastSeperatorIndex);
            AssetReferenceInfo directory;
            if(m_PathToAssetBundleDirectory.TryGetValue(filePath, out directory))
            {
                return directory;
            }
            return FindAssetBundleDirectory(filePath);
        }

        public AssetReferenceInfo CreateAssetInfo(string assetGuid)
        {
#if UNITY_EDITOR
            string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            var attributes = File.GetAttributes(assetPath);
            var importer = AssetImporter.GetAtPath(assetPath);
            string bundleName = importer.assetBundleName;

            if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                if (!string.IsNullOrEmpty(bundleName))
                {
                    return new AssetReferenceInfo()
                    {
                        Guid = assetGuid,
                        ReferenceType = AssetReferenceType.AssetBundleDirectory,
                        FilePath = assetPath,
                        BundleName = bundleName,
                        Varient = importer.assetBundleVariant
                    };
                }
                return null;
            }

            if (!string.IsNullOrEmpty(bundleName))
            {
                return new AssetReferenceInfo()
                {
                    Guid = assetGuid,
                    ReferenceType = AssetReferenceType.AssetBundle,
                    FilePath = assetPath,
                    BundleName = bundleName,
                    Varient = importer.assetBundleVariant
                };
            }
            if (assetPath.IndexOf(ResourcesWithSeperators) >= 0)
            {
                return new AssetReferenceInfo()
                {
                    Guid = assetGuid,
                    ReferenceType = AssetReferenceType.Resource,
                    FilePath = assetPath,
                };
            }

            //Detect asset bundle assets that are simply nested in asset bundle directories
            if (m_PathToAssetBundleDirectory.Count > 0)
            {
                var bundleDirectory = FindAssetBundleDirectory(assetPath);
                if (bundleDirectory != null)
                {
                    return new AssetReferenceInfo()
                    {
                        Guid = assetGuid,
                        ReferenceType = AssetReferenceType.AssetBundle,
                        FilePath = assetPath,
                        BundleName = bundleDirectory.BundleName,
                        Varient = bundleDirectory.Varient
                    };
                }
            }
            return null;
#else
            throw new NotSupportedException();
#endif
        }

        public void Rescan(bool withSave = true)
        {
#if UNITY_EDITOR
            m_GuidToAssetInfo.Clear();
            var allAssets = AssetDatabase.FindAssets(string.Empty);

            string lastGuid = string.Empty;
            for(int i = 0; i < allAssets.Length; ++i)
            {
                string currentGuid = allAssets[i];
                if (currentGuid != lastGuid)
                {
                    lastGuid = currentGuid;
                    UpdateAsset(currentGuid);
                }
            }

            if (withSave)
            {
                Save();
            }
            else
            {
                m_AssetInfoList = m_GuidToAssetInfo.Values.ToList();
                m_AssetBundleDirectories = m_PathToAssetBundleDirectory.Values.ToList();
            }
#else
            throw new NotSupportedException();
#endif
        }

        public void Save()
        {
#if UNITY_EDITOR
            m_AssetInfoList = m_GuidToAssetInfo.Values.ToList();
            m_AssetBundleDirectories = m_PathToAssetBundleDirectory.Values.ToList();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
#else
            throw new NotSupportedException();
#endif
        }

        public void OnBeforeSerialize()
        {
            //initial creation scan
            if(m_AssetInfoList == null)
            {
                Rescan(false);
            }
        }

        public void OnAfterDeserialize()
        {
            m_GuidToAssetInfo.Clear();
            m_PathToAssetBundleDirectory.Clear();

            int count = m_AssetInfoList.Count;
            for (int i = 0; i < count; ++i)
            {
                var info = m_AssetInfoList[i];
                m_GuidToAssetInfo[info.Guid] = info;
            }
            count = m_AssetBundleDirectories.Count;
            for (int i = 0; i < count; ++i)
            {
                var info = m_AssetBundleDirectories[i];
                m_PathToAssetBundleDirectory[info.FilePath] = info;
            }
        }
    }


#if UNITY_EDITOR

    public class AssetReferenceTrackerPostprocessor : UnityEditor.AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (AssetReferenceTracker.FindInstance() == null)
            {
                Debug.LogWarning("[AssetReferenceTracker] ScriptableObject not created!");
                return;
            }

            Debug.Log("[AssetReferenceTracker] Deleting " + deletedAssets.Length + " assets");

            //removing deleted assets
            bool needsToSave = false;
            foreach (var filePath in deletedAssets)
            {
                string guid = AssetDatabase.AssetPathToGUID(filePath);
                bool removed = AssetReferenceTracker.Instance.RemoveAsset(guid);
                if(!removed)
                {
                    removed = AssetReferenceTracker.Instance.RemoveDirectory(filePath);
                }
                needsToSave = needsToSave || removed;
            }


            //updating new or moved assets
            var updatingAssets = importedAssets.ToList();
            updatingAssets.Remove(AssetReferenceTracker.AssetReferenceTracherFilePath);
            updatingAssets.AddRange(movedAssets);
            Debug.Log("[AssetReferenceTracker] Updating " + updatingAssets.Count + " assets");

            foreach (var assetPath in updatingAssets)
            {
                needsToSave = AssetReferenceTracker.Instance.UpdateAsset(AssetDatabase.AssetPathToGUID(assetPath)) || needsToSave;
            }
            if (needsToSave)
            {
                AssetReferenceTracker.Instance.Save();
            }
        }

        public void OnPostprocessAssetbundleNameChanged(string assetPath, string previousAssetBundleName, string newAssetBundleName)
        {
            if (AssetReferenceTracker.FindInstance() == null)
            {
                Debug.LogWarning("[AssetReferenceTracker] ScriptableObject not created!");
                return;
            }
            Debug.Log("[AssetReferenceTracker] Asset " + assetPath + " has been moved from assetBundle " + previousAssetBundleName + " to assetBundle " + newAssetBundleName + ".");

            string assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
            var info = AssetReferenceTracker.Instance.GetAssetInfo(assetGuid);
            if (info != null && info.ReferenceType == AssetReferenceType.AssetBundleDirectory)
            {
                //rescan to adjust previous assets under this asset bundle directory
                Debug.Log("[AssetReferenceTracker] Change of a directory asset bundle requires a full rescan.");
                AssetReferenceTracker.Instance.Rescan();
            }
            else
            {
                //updating asset bundle information change
                AssetReferenceTracker.Instance.UpdateAsset(assetGuid);
            }
        }
    }
#endif
}
