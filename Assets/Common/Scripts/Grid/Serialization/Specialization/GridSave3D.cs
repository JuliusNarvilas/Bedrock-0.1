using Common.Utility;
#if UnityEditor
using UnityEditor;
#endif

namespace Common.Grid.Serialization.Specialization
{
    public class GridSave3D : GridSave<GridPosition3D, int>
    {
#if UnityEditor
        [MenuItem("Assets/Create/Grid/Save/3D")]
#endif
        public static void CreateAsset()
        {
            ScriptableObjectUtility.CreateAsset<GridSave3D>();
        }
    }
}
