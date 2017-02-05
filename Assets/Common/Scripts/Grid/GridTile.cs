﻿
namespace Common.Grid
{
    /// <summary>
    /// Representation of grid tile data and functionality.
    /// </summary>
    /// <typeparam name="TTerrain">The type of the terrain.</typeparam>
    /// <typeparam name="TPosition">The type of the position.</typeparam>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    public class GridTile<TTerrain, TPosition, TContext>
        where TTerrain : GridTerrain<TContext>
    {
        public readonly TPosition Position;
        public readonly TTerrain Terrain;

        public GridTile(TPosition i_Position, TTerrain i_Terrain)
        {
            Position = i_Position;
            Terrain = i_Terrain;
        }

        public virtual float GetTransitionInCost(GridTile<TTerrain, TPosition, TContext> i_FromTile, TContext i_Context)
        {
            return Terrain.GetCost(i_Context);
        }
        
        public virtual float GetTransitionOutCost(GridTile<TTerrain, TPosition, TContext> i_ToTile, TContext i_Context)
        {
            return 0.0f;
        }

        public virtual float GetDangerFactor()
        {
            return 0.0f;
        }

        public override string ToString()
        {
            return string.Format(
                "GridTile: { Position: {0} ; Terrain: {1} }",
                Position.ToString(),
                Terrain.ToString()
                );
        }
    }
}
