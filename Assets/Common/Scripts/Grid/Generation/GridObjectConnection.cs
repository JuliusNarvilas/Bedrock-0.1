using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Grid.Generation
{
    public enum EConnectionSettings : int
    {
        Required = 1 << 3, //8
        Inward = 1 << 4,
        Outward = 1 << 5
    }

    public struct GridObjectConnection3D
    {
        public GridPosition3D LocalPosition;
        public int Settings;
    }

    public struct GridObjectConnection2D
    {
        public GridPosition2D LocalPosition;
        public int Settings;
    }
}
