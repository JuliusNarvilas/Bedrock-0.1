
using System;

namespace Common.Grid
{
    public struct GridPosition2D : IEquatable<GridPosition2D>
    {
        public readonly int X;
        public readonly int Y;

        public GridPosition2D(int i_IndexX = 0, int i_IndexY = 0)
        {
            X = i_IndexX;
            Y = i_IndexY;
        }

        public bool Equals(GridPosition2D i_Other)
        {
            return X == i_Other.X && Y == i_Other.Y;
        }

        public override string ToString()
        {
            return string.Format("{ X: {0}; Y: {1}}", X, Y);
        }
    }
}
