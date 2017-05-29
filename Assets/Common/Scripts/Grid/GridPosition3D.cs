
using System;

namespace Common.Grid
{
    /// <summary>
    /// Structure for 3D positioning coordinates.
    /// </summary>
    /// <seealso cref="System.IEquatable{Common.Grid.GridPosition3D}" />
    [Serializable]
    public struct GridPosition3D : IEquatable<GridPosition3D>
    {
        public int X;
        public int Y;
        public int Z;

        public GridPosition3D(int i_IndexX = 0, int i_IndexY = 0, int i_IndexZ = 0)
        {
            X = i_IndexX;
            Y = i_IndexY;
            Z = i_IndexZ;
        }

        public bool Equals(GridPosition3D i_Other)
        {
            return X == i_Other.X && Y == i_Other.Y && Z == i_Other.Z;
        }

        public override string ToString()
        {
            return string.Format("{{ X: {0}; Y: {1}; Z: {2}}", X, Y, Z);
        }

        public static GridPosition3D operator+ (GridPosition3D i_ObjA, GridPosition3D i_ObjB)
        {
            return new GridPosition3D(i_ObjA.X + i_ObjB.X, i_ObjA.Y + i_ObjB.Y, i_ObjA.Z + i_ObjB.Z);
        }

        public static GridPosition3D operator -(GridPosition3D i_ObjA, GridPosition3D i_ObjB)
        {
            return new GridPosition3D(i_ObjA.X - i_ObjB.X, i_ObjA.Y - i_ObjB.Y, i_ObjA.Z - i_ObjB.Z);
        }
    }
}
