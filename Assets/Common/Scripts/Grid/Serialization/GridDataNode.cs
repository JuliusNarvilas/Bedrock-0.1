using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Common.Grid.Serialization
{
    public class GridDataNode
    {
        public GridTransform Transform;
        public GridPosition3D Min;
        public GridPosition3D Max;

        public List<GridDataModel> Models;
        public List<GridDataNode> Children;
    }
}
