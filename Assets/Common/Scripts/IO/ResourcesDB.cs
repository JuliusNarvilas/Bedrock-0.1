using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Common.IO
{
    public class ResourcesDB : ScriptableObject, ISerializationCallbackReceiver
    {
        private static readonly string RESOURCES_STR = "Resources";
        private static ResourcesDB s_Instance = null;
        public static ResourcesDB FindInstance()
        {
            return Resources.Load<ResourcesDB>("ResourceDB");
        }
        public ResourcesDB()
        {
            s_Instance = this;
        }
        public static ResourcesDB Instance
        {
            get
            {
                if (s_Instance != null)
                    return s_Instance;
                s_Instance = FindInstance();
                if (s_Instance != null)
                    return s_Instance;
                s_Instance = CreateInstance<ResourcesDB>();
#if UNITY_EDITOR
                var resDir = new DirectoryInfo(Path.Combine(Application.dataPath, RESOURCES_STR));
                if (!resDir.Exists)
                    UnityEditor.AssetDatabase.CreateFolder("Assets", RESOURCES_STR);
                UnityEditor.AssetDatabase.CreateAsset(s_Instance, "Assets/Resources/ResourceDB.asset");
                s_Instance = FindInstance();
#endif
                return s_Instance;
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/Update ResourceDB")]
        internal static void TriggerUpdate()
        {
            Instance.UpdateDB();
        }
#endif

        [SerializeField]
        internal List<ResourcesDBItem> m_Items = new List<ResourcesDBItem>();

        [SerializeField, HideInInspector]
        private int m_FileCount = 0;

        [SerializeField, HideInInspector]
        private int m_FolderCount = 0;

        [SerializeField, HideInInspector]
        public bool UpdateAutomatically = false;

        internal ResourcesDBItem root = new ResourcesDBItem("", "", ResourcesDBItem.Type.Folder, "");

        public int FileCount { get { return m_FileCount; } }
        public int FolderCount { get { return m_FolderCount; } }

        public static ResourcesDBItem GetFolder(string i_Path)
        {
            return Instance.root.GetChild(i_Path, ResourcesDBItem.Type.Folder);
        }
        public static IEnumerable<ResourcesDBItem> GetAllAssets(string i_Name, System.Type i_AssetType = null)
        {
            return Instance.root.GetChilds(i_Name, ResourcesDBItem.Type.Asset, true, i_AssetType);
        }
        public static IEnumerable<ResourcesDBItem> GetAllAssets<T>(string i_Name) where T : UnityEngine.Object
        {
            return GetAllAssets(i_Name, typeof(T));
        }
        public static ResourcesDBItem GetAsset(string i_Name, System.Type i_AssetType = null)
        {
            return Instance.root.GetChilds(i_Name, ResourcesDBItem.Type.Asset, true, i_AssetType).FirstOrDefault();
        }

        private static int IndexAfterResources(string i_Path, int i_StartingIndex = 0)
        {

            int indexOfResources = i_Path.IndexOf(RESOURCES_STR, i_StartingIndex, System.StringComparison.InvariantCultureIgnoreCase);
            if (indexOfResources >= 0)
            {
                int resourcesEndIndex = indexOfResources + RESOURCES_STR.Length;
                if((indexOfResources > 0) && (i_Path[indexOfResources - 1] != '/'))
                {
                    return IndexAfterResources(i_Path, resourcesEndIndex - 1);
                }
                if ((i_Path.Length - resourcesEndIndex) > 0)
                {
                    if(i_Path[resourcesEndIndex] != '/')
                    {
                        return IndexAfterResources(i_Path, resourcesEndIndex + 1);
                    }
                    ++resourcesEndIndex;
                }
                return resourcesEndIndex;
            }
            return -1;
        }

        public static ResourcesDBItem GetByPath(string i_Path, ResourcesDBItem.Type i_ResourceType = ResourcesDBItem.Type.Any)
        {
            if(string.IsNullOrEmpty(i_Path))
            {
                return null;
            }
            int substringStart = IndexAfterResources(i_Path);
            if(substringStart >= 0)
            {
                i_Path = i_Path.Substring(substringStart);
            }
            return GetByResourcesPath(i_Path, i_ResourceType);
        }

        public static ResourcesDBItem GetByResourcesPath(string i_Path, ResourcesDBItem.Type i_ResourceType = ResourcesDBItem.Type.Any)
        {
            return Instance.root.GetChild(i_Path, i_ResourceType);
        }

        public static string ConvertPath(string aPath)
        {
            return aPath.Replace("\\", "/");
        }

#if UNITY_EDITOR
        private void ScanFolderForResourcesDirectories(DirectoryInfo i_Folder, List<DirectoryInfo> i_List, bool aOnlyTopFolders)
        {
            string folderName = i_Folder.Name.ToLower();
            if (folderName == "editor") // ignore folders
                return;
            if (folderName == "resources")
            {
                i_List.Add(i_Folder);
                if (aOnlyTopFolders)
                    return;
            }
            var directories = i_Folder.GetDirectories();
            int size = directories.Length;
            for (int i = 0; i < size; ++i)
            {
                ScanFolderForResourcesDirectories(directories[i], i_List, aOnlyTopFolders);
            }
        }
        private List<DirectoryInfo> FindResourcesFolders(bool aOnlyTopFolders)
        {
            var assetsDirectory = new DirectoryInfo(Application.dataPath);
            var list = new List<DirectoryInfo>();
            ScanFolderForResourcesDirectories(assetsDirectory, list, aOnlyTopFolders);
            return list;
        }

        private void AddFileList(DirectoryInfo i_Folder, int i_Prefix)
        {
            string relFolder = i_Folder.FullName;
            if (relFolder.Length < i_Prefix)
                relFolder = string.Empty;
            else
                relFolder = relFolder.Substring(i_Prefix);
            relFolder = ConvertPath(relFolder);

            var directories = i_Folder.GetDirectories();
            int size = directories.Length;
            DirectoryInfo folder;
            for(int i = 0; i < size; ++i)
            {
                folder = directories[i];
                m_Items.Add(new ResourcesDBItem(folder.Name, relFolder, ResourcesDBItem.Type.Folder, string.Empty));
                AddFileList(folder, i_Prefix);
            }

            var files = i_Folder.GetFiles();
            size = files.Length;
            FileInfo file;
            for(int i = 0; i < size; ++i)
            {
                file = files[i];
                string ext = file.Extension.ToLower();
                if (ext == ".meta")
                    continue;
                string assetPath = "assets/" + file.FullName.Substring(Application.dataPath.Length + 1);
                assetPath = ConvertPath(assetPath);
                var obj = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
                if (obj == null)
                {
                    Log.ProductionLogWarning("ResourceDB: File at path {0} couldn't be loaded and is ignored. Probably not an asset?!", assetPath);
                    continue;
                }
                string type = obj.GetType().AssemblyQualifiedName;
                m_Items.Add(new ResourcesDBItem(file.Name, relFolder, ResourcesDBItem.Type.Asset, type));
            }
            Resources.UnloadUnusedAssets();
        }

        public void UpdateDB(bool i_SetDirty = false)
        {
            m_Items.Clear();
            root.m_Children.Clear();
            var topFolders = FindResourcesFolders(true);

            int size = topFolders.Count;
            DirectoryInfo folder;
            for(int i = 0; i < size; ++i)
            {
                folder = topFolders[i];
                string path = folder.FullName;
                int prefix = path.Length;
                if (!path.EndsWith("/"))
                    prefix++;
                AddFileList(folder, prefix);
            }
            m_FolderCount = 0;
            m_FileCount = 0;

            size = m_Items.Count;
            ResourcesDBItem.Type resourceType;
            for (int i = 0; i < size; ++i)
            {
                resourceType = m_Items[i].ResourcesType;
                if (resourceType == ResourcesDBItem.Type.Folder)
                    m_FolderCount++;
                else if (resourceType == ResourcesDBItem.Type.Asset)
                    m_FileCount++;
            }
            if (i_SetDirty)
            {
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.AssetDatabase.SaveAssets();
            }
        }
#endif

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (m_Items == null || m_Items.Count == 0)
            {
                UpdateDB();
            }
#endif
        }

        public void OnAfterDeserialize()
        {
            root.m_Children.Clear();
            int size = m_Items.Count;
            ResourcesDBItem item;
            for (int i = 0; i < size; ++i)
            {
                item = m_Items[i];
                if (item != null)
                    item.OnDeserialize();
            }
        }
    }


#if UNITY_EDITOR

    public class ResourceDBPostprocessor : UnityEditor.AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (ResourcesDB.FindInstance() == null)
                return;
            if (!ResourcesDB.Instance.UpdateAutomatically)
                return;
            var files = importedAssets.Concat(deletedAssets).Concat(movedAssets).Concat(movedFromAssetPaths);
            bool update = false;

            foreach (var file in files)
            {
                var fn = file.ToLower();
                if (!fn.Contains("resourcedb.asset") && fn.Contains("/resources/"))
                {
                    update = true;
                    break;
                }
            }
            if (update)
            {
                ResourcesDB.Instance.UpdateDB();
            }
        }
    }
#endif
}