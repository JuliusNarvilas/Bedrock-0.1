using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Grid.Serialization
{
    [Serializable]
    public struct GridTransform
    {
        public GridPosition3D Translation;
        public int OrientationId;
    }
}
