using System;
using System.Collections.Generic;

namespace Common.Grid.Path
{
    /// <summary>
    /// A pool of reusable <see cref="GridPathElement{TPosition, TContext, TTile}"/> instances.
    /// </summary>
    public class GridPathElementPool<TPosition, TContext, TTile> : ObjectPool<GridPathElement<TPosition, TContext, TTile>> where TTile : GridTile<TPosition, TContext, TTile>
    {
        public static readonly GridPathElementPool<TPosition, TContext, TTile> GLOBAL = new GridPathElementPool<TPosition, TContext, TTile>(200);

        public GridPathElementPool(int i_Capacity) : base(i_Capacity)
        {
        }

        public override GridPathElement<TPosition, TContext, TTile> Create()
        {
            return new GridPathElement<TPosition, TContext, TTile>();
        }
    }
}
