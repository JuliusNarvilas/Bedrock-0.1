
using System;

namespace Common.Grid
{
    /// <summary>
    /// Structure for 2D positioning coordinates.
    /// </summary>
    /// <seealso cref="System.IEquatable{Common.Grid.GridPosition2D}" />
    [Serializable]
    public struct GridPosition2D : IEquatable<GridPosition2D>
    {
        public int X;
        public int Y;

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

        public static GridPosition2D operator +(GridPosition2D i_ObjA, GridPosition2D i_ObjB)
        {
            return new GridPosition2D(i_ObjA.X + i_ObjB.X, i_ObjA.Y + i_ObjB.Y);
        }

        public static GridPosition2D operator -(GridPosition2D i_ObjA, GridPosition2D i_ObjB)
        {
            return new GridPosition2D(i_ObjA.X - i_ObjB.X, i_ObjA.Y - i_ObjB.Y);
        }
    }
}
