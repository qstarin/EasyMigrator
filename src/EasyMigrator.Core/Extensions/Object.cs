using System;
using System.Collections.Generic;
using System.Linq;



namespace EasyMigrator
{
    /// <summary>
    /// Defines extension methods for the System.Object type.
    /// </summary>
    internal static class ObjectExtensions
    {
        /// <summary>
        /// Tests whether the object is not null.  If not null, invokes the given delegate.
        /// </summary>
        /// <remarks>
        /// This extension method is particularly useful for invoking event delegates while maintaining thread-safety.
        /// </remarks>
        /// <typeparam name="T">Type of the extension target object.</typeparam>
        /// <param name="obj">Extension target object.</param>
        /// <param name="then">Delegate to invoke if the object is not null.</param>
        public static void IfNotNull<T>(this T obj, Action<T> then)
            where T : class
        {
            if (obj != null) { then(obj); }
        }

        /// <summary>
        /// Tests whether the object is not null.  If not null, invokes the given delegate and returns its value; otherwise, returns null.
        /// </summary>
        /// <typeparam name="T">Type of the extension target object.</typeparam>
        /// <typeparam name="TResult">Type of value to return.</typeparam>
        /// <param name="obj">Extension target object.</param>
        /// <param name="then">Delegate to invoke if the object is not null.</param>
        /// <returns>If the object is not null, the value returned by the given delegate; otherwise, null.</returns>
        public static TResult IfNotNull<T, TResult>(this T obj, Func<T, TResult> then)
            where T : class
            where TResult : class
        {
            return obj != null ? then(obj) : null;
        }

        /// <summary>
        /// Tests whether the object is not null.  If not null, invokes the given delegate and returns its value; otherwise, returns the given value.
        /// </summary>
        /// <typeparam name="T">Type of the extension target object.</typeparam>
        /// <typeparam name="TResult">Type of value to return.</typeparam>
        /// <param name="obj">Extension target object.</param>
        /// <param name="then">Delegate to invoke if the object is not null.</param>
        /// <param name="otherwise">Value to return if the object is null.</param>
        /// <returns>If the object is not null, the value returned by the given delegate; otherwise, the given value.</returns>
        public static TResult IfNotNull<T, TResult>(this T obj, Func<T, TResult> then, TResult otherwise)
            where T : class
        {
            return obj != null ? then(obj) : otherwise;
        }

        /// <summary>
        /// Tests whether the object is not null.  If not null, invokes the first given delegate; otherwise, invokes the second given delegate.
        /// </summary>
        /// <typeparam name="T">Type of the extension target object.</typeparam>
        /// <typeparam name="TResult">Type of value to return.</typeparam>
        /// <param name="obj">Extension target object.</param>
        /// <param name="then">Delegate to invoke if the object is not null.</param>
        /// <param name="otherwise">Delegate to invoke if the object is null.</param>
        /// <returns>If the object is not null, the value returned by the first delegate; otherwise, the value returned by the second delegate.</returns>
        public static TResult IfNotNull<T, TResult>(this T obj, Func<T, TResult> then, Func<TResult> otherwise)
            where T : class
        {
            return obj != null ? then(obj) : otherwise();
        }

        /// <summary>
        /// Tests whether the object is null.  If null, returns the given value; otherwise, invokes the given delegate.
        /// </summary>
        /// <typeparam name="T">Type of the extension target object.</typeparam>
        /// <typeparam name="TResult">Type of value to return.</typeparam>
        /// <param name="obj">Extension target object.</param>
        /// <param name="then">Value to return if the object is null.</param>
        /// <param name="otherwise">Delegate to invoke if the object is not null.</param>
        /// <returns>If the object is null, the given value; otherwise, the value returned by the given delegate.</returns>
        public static TResult IfNull<T, TResult>(this T obj, TResult then, Func<T, TResult> otherwise)
            where T : class
        {
            return obj == null ? then : otherwise(obj);
        }

        /// <summary>
        /// Tests whether the object is null.  If null, invokes the first given delegate; otherwise, invokes the second given delegate.
        /// </summary>
        /// <typeparam name="T">Type of the extension target object.</typeparam>
        /// <typeparam name="TResult">Type of value to return.</typeparam>
        /// <param name="obj">Extension target object.</param>
        /// <param name="then">Delegate to invoke if the object is null.</param>
        /// <param name="otherwise">Delegate to invoke if the object is not null.</param>
        /// <returns>If the object is null, the value returned by the first given delegate; otherwise, the value returned by the second given delegate.</returns>
        public static TResult IfNull<T, TResult>(this T obj, Func<TResult> then, Func<T, TResult> otherwise)
            where T : class
        {
            return obj == null ? then() : otherwise(obj);
        }

    }
}
