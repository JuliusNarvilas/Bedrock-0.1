using Common.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Common.Grid.Serialization.Specialization
{
    public class GridSave3D : GridSave<GridPosition3D, int>
    {
        [MenuItem("Assets/Create/Grid/Save/3D")]
        public static void CreateAsset()
        {
            ScriptableObjectUtility.CreateAsset<GridSave3D>();
        }
    }
}
