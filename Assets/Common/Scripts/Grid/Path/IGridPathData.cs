using System;

namespace Common.Grid.Path
{
    public enum GridPathDataResponse
    {
        InvalidPosition,
        OutOfDataRange,
        Success
    }

    public interface IGridPathDataProvider<TTile, TTerrain, TPosition, TContext>
        where TTile : GridTile<TTerrain, TPosition, TContext>
        where TTerrain : GridTerrain<TContext>
    {
        IGridPathData<TTile, TTerrain, TPosition, TContext> GetGridPathData(TPosition i_Size);

        void Recycle(IGridPathData<TTile, TTerrain, TPosition, TContext> i_Data);
    }

    public interface IGridPathData<TTile, TTerrain, TPosition, TContext> : IDisposable
        where TTile : GridTile<TTerrain, TPosition, TContext>
        where TTerrain : GridTerrain<TContext>
    {
        GridPathDataResponse TryGetElement(TPosition i_Pos, out GridPathElement<TTile, TTerrain, TPosition, TContext> o_Value);

        bool Grow(TPosition i_EnvelopPos);

        void Clean();
    }
}
