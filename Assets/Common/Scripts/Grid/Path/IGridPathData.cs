using System;

namespace Common.Grid.Path
{
    public enum GridPathDataResponse
    {
        InvalidPosition,
        OutOfDataRange,
        Success
    }

    /// <summary>
    /// A manager interface that maintains <see cref="IGridPathData{TTile, TTerrain, TPosition, TContext}"/> instances for reusability.
    /// </summary>
    /// <typeparam name="TTile">The type of the tile.</typeparam>
    /// <typeparam name="TTerrain">The type of the terrain.</typeparam>
    /// <typeparam name="TPosition">The type of the position.</typeparam>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    public interface IGridPathDataProvider<TTile, TTerrain, TPosition, TContext>
        where TTile : GridTile<TTerrain, TPosition, TContext>
        where TTerrain : GridTerrain<TContext>
    {
        /// <summary>
        /// Gets the grid path data.
        /// </summary>
        /// <returns>Grid pathing data cache.</returns>
        IGridPathData<TTile, TTerrain, TPosition, TContext> GetGridPathData();

        /// <summary>
        /// Recycles the specified path data.
        /// </summary>
        /// <param name="i_Data">The pathing data cache.</param>
        void Recycle(IGridPathData<TTile, TTerrain, TPosition, TContext> i_Data);
    }

    /// <summary>
    /// Interface for the grid pathing data caching.
    /// This provides a basic interface for supporting resizable pathing data cache when operating on a large grid section.
    /// </summary>
    /// <typeparam name="TTile">The type of the tile.</typeparam>
    /// <typeparam name="TTerrain">The type of the terrain.</typeparam>
    /// <typeparam name="TPosition">The type of the position.</typeparam>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    /// <seealso cref="System.IDisposable" />
    public interface IGridPathData<TTile, TTerrain, TPosition, TContext> : IDisposable
        where TTile : GridTile<TTerrain, TPosition, TContext>
        where TTerrain : GridTerrain<TContext>
    {
        /// <summary>
        /// Sets the initial pathing data bounds.
        /// </summary>
        /// <param name="i_Source">The i source.</param>
        /// <param name="i_Min">The i minimum.</param>
        /// <param name="i_Max">The i maximum.</param>
        void Set(IGridControl<TTile, TTerrain, TPosition, TContext> i_Source, TPosition i_Min, TPosition i_Max);
        
        /// <summary>
        /// Attempts to get cached pathing data.
        /// </summary>
        /// <param name="i_Pos">The position.</param>
        /// <param name="o_Value">Pathing data result.</param>
        /// <returns>Success or failure type.</returns>
        GridPathDataResponse TryGetElement(TPosition i_Pos, out GridPathElement<TTile, TTerrain, TPosition, TContext> o_Value);

        /// <summary>
        /// Grows the specified i envelop position.
        /// </summary>
        /// <param name="i_EnvelopPos">The i envelop position.</param>
        /// <returns></returns>
        bool Grow(TPosition i_EnvelopPos);

        /// <summary>
        /// Cleans this instance for later pathing use.
        /// </summary>
        void Clean();
    }
}
