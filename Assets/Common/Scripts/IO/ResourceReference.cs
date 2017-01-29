using System;

namespace Common.IO
{
    [Serializable]
    public class ResourceReference
    {
        /// <summary>
        /// The file path that acts as a Resources asset reference.
        /// </summary>
        public string FilePath;
        /// <summary>
        /// For nested assets like Sprites.
        /// </summary>
        public string SubName;
        /// <summary>
        /// For storing nested asset type information.
        /// </summary>
        public string SubType;

        public T Get<T>() where T : UnityEngine.Object
        {
            var temp = ResourcesDB.GetByPath(FilePath);
            if(temp != null)
            {
                if (!string.IsNullOrEmpty(SubName))
                {
                    return temp.Load(Type.GetType(SubType), SubName) as T;
                }
                return temp.Load<T>();
            }
            return null;
        }
    }
}
