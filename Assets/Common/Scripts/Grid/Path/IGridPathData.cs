using System;

namespace Common.Grid.Path
{
    public enum EGridPathDataResponse
    {
        InvalidPosition,
        OutOfDataRange,
        Success
    }

    /// <summary>
    /// Interface for the grid pathing data caching.
    /// This provides a basic interface for supporting resizable pathing data cache when operating on a large grid section.
    /// </summary>
    /// <typeparam name="TPosition">The type of the position.</typeparam>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    /// <seealso cref="System.IDisposable" />
    public interface IGridPathData<TPosition, TContext, TTile> : IDisposable where TTile : GridTile<TPosition, TContext, TTile>
    {

        TPosition GetMin();
        TPosition GetMax();

        /// <summary>
        /// Sets the initial pathing data bounds.
        /// </summary>
        /// <param name="i_Source">The i source.</param>
        /// <param name="i_Min">The i minimum.</param>
        /// <param name="i_Max">The i maximum.</param>
        bool Set(IGridControl<TPosition, TContext, TTile> i_Source, TPosition i_Min, TPosition i_Max);
        
        /// <summary>
        /// Attempts to get cached pathing data.
        /// </summary>
        /// <param name="i_Pos">The position.</param>
        /// <param name="o_Value">Pathing data result.</param>
        /// <returns>Success or failure type.</returns>
        EGridPathDataResponse TryGetElement(TPosition i_Pos, out GridPathElement<TPosition, TContext, TTile> o_Value);

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
        /// Called by the <see cref="GridPathDataProvider{TPosition, TContext, TTile}"/> when it decides to get rid of this instance from being cached.
        /// </summary>
        void OnDestroy();
    }
}
