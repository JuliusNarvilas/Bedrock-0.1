﻿using System;

namespace Common.Properties.Numerical.Data
{
    /// <summary>
    /// Interface for any <see cref="NumericalProperty{TNumerical, TContext, TModifierReader}"/> data type with required numerical functionality.
    /// </summary>
    /// <typeparam name="TNumerical">The type of the underlying numerical data.</typeparam>
    /// <seealso cref="System.IComparable{TNumerical}" />
    public interface INumericalPropertyData<TNumerical> : IComparable<TNumerical>
    {
        /// <summary>
        /// Gets the stored underlying numerical data.
        /// </summary>
        /// <returns>The underlying value.</returns>
        TNumerical Get();
        /// <summary>
        /// Sets the stored underlying numerical data.
        /// </summary>
        /// <param name="i_Value">The new value.</param>
        void Set(TNumerical i_Value);
        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="i_Value">The add amount.</param>
        void Add(TNumerical i_Value);
        /// <summary>
        /// Substracts the specified value.
        /// </summary>
        /// <param name="i_Value">The substract amount.</param>
        void Substract(TNumerical i_Value);
        /// <summary>
        /// Multiplies the specified value.
        /// </summary>
        /// <param name="i_Value">The multiply amount.</param>
        void Multiply(TNumerical i_Value);
        /// <summary>
        /// Divides the specified value.
        /// </summary>
        /// <param name="i_Value">The divide amount.</param>
        void Divide(TNumerical i_Value);
        /// <summary>
        /// Gets the additive inverse of the underlying data, so that <code>value + x = 0</code>.
        /// </summary>
        /// <returns>The additive inverse.</returns>
        TNumerical AdditiveInverse();
        /// <summary>
        /// Gets the multiplicative inverse of the underlying data, so that <code>value * x = 1 (identity)</code>.
        /// </summary>
        /// <returns>The multiplicative inverse.</returns>
        TNumerical MultiplicativeInverse();
        /// <summary>
        /// Sets the underlying data to a zero representation.
        /// </summary>
        void ToZero();
        /// <summary>
        /// Creates a new instance of this data type with an underlying data representation of zero.
        /// </summary>
        /// <returns>New instance with data representing zero.</returns>
        INumericalPropertyData<TNumerical> CreateZero();
    }

    public static class NumericalPropertyDataExtensions
    {
        public static void Set<T>(this INumericalPropertyData<T> i_Target, INumericalPropertyData<T> i_Value)
        {
            i_Target.Set(i_Value.Get());
        }
        public static void Add<T>(this INumericalPropertyData<T> i_Target, INumericalPropertyData<T> i_Value)
        {
            i_Target.Add(i_Value.Get());
        }
        public static void Substract<T>(this INumericalPropertyData<T> i_Target, INumericalPropertyData<T> i_Value)
        {
            i_Target.Substract(i_Value.Get());
        }
        public static void Multiply<T>(this INumericalPropertyData<T> i_Target, INumericalPropertyData<T> i_Value)
        {
            i_Target.Multiply(i_Value.Get());
        }
        public static void Divide<T>(this INumericalPropertyData<T> i_Target, INumericalPropertyData<T> i_Value)
        {
            i_Target.Divide(i_Value.Get());
        }
    }
    
}
