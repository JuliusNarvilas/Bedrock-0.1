using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Common.IO
{
    [System.Serializable]
    public class ResourcesDBItem
    {
        public enum Type
        {
            Unknown = 0,
            Any = 0,
            Folder = 1,
            Asset = 2,
        }

        /// <summary>
        /// The file name without the extention.
        /// </summary>
        [SerializeField]
        private string m_Name;

        /// <summary>
        /// The extention of the file.
        /// </summary>
        [SerializeField]
        private string m_Ext;

        /// <summary>
        /// The path to the folder, containing the file.
        /// </summary>
        [SerializeField]
        private string m_Path;

        [SerializeField]
        private Type m_Type = Type.Unknown;

        [SerializeField]
        private string m_ObjectTypeName;

        [System.NonSerialized]
        private int m_RefCount = 0;

        [System.NonSerialized]
        private List<Object> m_Cache = new List<Object>();

        private System.Type m_ObjectType;

        private ResourcesDBItem m_Parent = null;

        internal Dictionary<string, ResourcesDBItem> m_Children = null;

        public string Name { get { return m_Name; } }

        public string Ext { get { return m_Ext; } }

        public string Path { get { return m_Path; } }

        public string ResourcesPath { get { return string.IsNullOrEmpty(m_Path) ? m_Name : m_Path + "/" + m_Name; } }

        public Type ResourcesType { get { return m_Type; } }

        public ResourcesDBItem Parent { get { return m_Parent; } }

        public ResourcesDBItem()
        {
            if (m_Type == Type.Folder)
                m_Children = new Dictionary<string, ResourcesDBItem>();
        }

        public ResourcesDBItem(string i_FileName, string i_Path, Type i_Type, string i_ObjectType)
        {
            m_Ext = System.IO.Path.GetExtension(i_FileName);
            m_Name = System.IO.Path.ChangeExtension(i_FileName, null);

            m_Path = i_Path;
            m_Type = i_Type;
            m_ObjectTypeName = i_ObjectType;
            m_ObjectType = System.Type.GetType(m_ObjectTypeName);
            if (m_Type == Type.Folder)
                m_Children = new Dictionary<string, ResourcesDBItem>();
        }

        public ResourcesDBItem GetChild(string i_Path, Type i_ResourceType = Type.Any)
        {
            if (m_Type != Type.Folder)
                return null;

            string childPath = i_Path;
            int index = i_Path.IndexOf('/');
            if (index > 0)
            {
                childPath = i_Path.Substring(0, index);
                i_Path = i_Path.Substring(index + 1);
            }
            else
                i_Path = string.Empty;

            ResourcesDBItem item = null;
            if (!m_Children.TryGetValue(childPath, out item) || item == null)
                return null;
            if (i_Path.Length > 0)
                return item.GetChild(i_Path, i_ResourceType);
            if (i_ResourceType != Type.Unknown && item.m_Type != i_ResourceType)
                return null;
            return item;
        }

        public IEnumerable<ResourcesDBItem> GetChilds(string i_Name, Type i_ResourceType = Type.Any, bool i_SearchSubFolders = false, System.Type i_AssetType = null)
        {
            if (m_Type == Type.Asset) // assets don't have childs
                yield break;

            bool checkName = !string.IsNullOrEmpty(i_Name);
            bool typeCheck = i_AssetType != null;
            var items = m_Children.Values;
            foreach (var item in items)
            {
                if (i_ResourceType != Type.Any && item.m_Type != i_ResourceType)
                    continue;
                if (checkName && i_Name != item.Name)
                    continue;
                if (typeCheck && !i_AssetType.IsAssignableFrom(item.m_ObjectType))
                    continue;
                yield return item;
            }
            if (i_SearchSubFolders)
            {
                foreach (var folder in items.Where(i => i.m_Type == Type.Folder))
                {
                    foreach (var item in folder.GetChilds(i_Name, i_ResourceType, i_SearchSubFolders, i_AssetType))
                        yield return item;
                }
            }
        }


        public T Load<T>(string i_SubName = null) where T : UnityEngine.Object
        {
            return Load(typeof(T), i_SubName) as T;
        }

        public Object Load(System.Type i_Type, string i_SubName = null)
        {
            ++m_RefCount;
            if (m_Cache.Count <= 0)
            {
                m_Cache.AddRange(Resources.LoadAll(ResourcesPath, i_Type));
                if (!string.IsNullOrEmpty(i_SubName))
                {
                    if (i_SubName != m_Name)
                    {
                        m_Cache.Add(Resources.Load(ResourcesPath));
                    }
                }
            }
            if (string.IsNullOrEmpty(i_SubName))
            {
                return m_Cache.Last();
            }
            else
            {
                foreach (var cacheItem in m_Cache)
                {
                    if (cacheItem.name == i_SubName)
                    {
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
                foreach (var cacheItem in m_Cache)
                {
                    Resources.UnloadAsset(cacheItem);
                }
                m_RefCount = 0;
                m_Cache.Clear();
                Resources.UnloadUnusedAssets();
            }
        }


        internal void OnDeserialize()
        {
            if (string.IsNullOrEmpty(m_Path))
                m_Parent = ResourcesDB.Instance.root;
            else
                m_Parent = ResourcesDB.GetFolder(m_Path);
            if (m_Parent != null)
                m_Parent.m_Children.Add(m_Name, this);
            if (m_Type == Type.Folder)
            {
                m_Children = new Dictionary<string, ResourcesDBItem>();
            }
            m_ObjectType = System.Type.GetType(m_ObjectTypeName);
        }
    }
}