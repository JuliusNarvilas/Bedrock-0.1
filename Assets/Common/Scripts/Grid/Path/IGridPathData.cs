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
    /// Interface for the grid pathing data caching.
    /// This provides a basic interface for supporting resizable pathing data cache when operating on a large grid section.
    /// </summary>
    /// <typeparam name="TTileData">The type of the tile data.</typeparam>
    /// <typeparam name="TPosition">The type of the position.</typeparam>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    /// <seealso cref="System.IDisposable" />
    public interface IGridPathData<TPosition, TTileData, TContext> : IDisposable
    {
        /// <summary>
        /// Sets the initial pathing data bounds.
        /// </summary>
        /// <param name="i_Source">The i source.</param>
        /// <param name="i_Min">The i minimum.</param>
        /// <param name="i_Max">The i maximum.</param>
        bool Set(IGridControl<TPosition, TTileData, TContext> i_Source, TPosition i_Min, TPosition i_Max);
        
        /// <summary>
        /// Attempts to get cached pathing data.
        /// </summary>
        /// <param name="i_Pos">The position.</param>
        /// <param name="o_Value">Pathing data result.</param>
        /// <returns>Success or failure type.</returns>
        GridPathDataResponse TryGetElement(TPosition i_Pos, out GridPathElement<TPosition, TTileData, TContext> o_Value);

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

        /// <summary>
        /// Called by the <see cref="GridPathDataProvider{TPosition, TTileData, TContext}"/> when it decides to get rid of this instance from being cached.
        /// </summary>
        void OnDestroy();
    }
}
