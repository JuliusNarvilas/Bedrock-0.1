
using System;

namespace Common.Grid
{
    /// <summary>
    /// Immutable structure for 2D positioning coordinates.
    /// </summary>
    /// <seealso cref="System.IEquatable{Common.Grid.GridPosition3D}" />
    public struct GridPosition3D : IEquatable<GridPosition3D>
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;

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
    }
}
