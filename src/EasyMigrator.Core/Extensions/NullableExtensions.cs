using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMigrator.Extensions
{
    /// <summary>
    /// Defines extension methods for the System.Nullable&lt;T&gt; type.
    /// </summary>
    public static class NullableExtensions
    {
        /// <summary>
        /// Tests whether the nullable object has a value.  If so, invokes the given delegate.
        /// </summary>
        /// <typeparam name="T">Underlying type of the nullable object.</typeparam>
        /// <param name="obj">Extension target object.</param>
        /// <param name="then">Delegate to invoke if the nullable object has a value.</param>
        public static void IfHasValue<T>(this T? obj, Action<T> then)
            where T : struct
        {
            if (obj.HasValue)
                then(obj.Value);
        }

        /// <summary>
        /// Tests whether the nullable object has a value.  If so, invokes the given delegate and returns its value; otherwise, returns null.
        /// </summary>
        /// <typeparam name="T">Underlying type of the nullable object.</typeparam>
        /// <typeparam name="TResult">Type of value to return.</typeparam>
        /// <param name="obj">Extension target object.</param>
        /// <param name="then">Delegate to invoke if the nullable object has a value.</param>
        /// <returns>If the nullable object has a value, the value returned from the given delegate; otherwise, null.</returns>
        public static TResult IfHasValue<T, TResult>(this T? obj, Func<T, TResult> then)
            where T : struct
        {
            return obj.HasValue ? then(obj.Value) : default(TResult);
        }

        /// <summary>
        /// Tests whether the nullable object has a value.  If so, invokes the given delegate and returns its value; otherwise, returns the value given.
        /// </summary>
        /// <typeparam name="T">Underlying type of the nullable object.</typeparam>
        /// <typeparam name="TResult">Type of value to return.</typeparam>
        /// <param name="obj">Extension target object.</param>
        /// <param name="then">Delegate to invoke if the nullable object has a value.</param>
        /// <param name="otherwise">Value to return if the nullable object does not have a value.</param>
        /// <returns>If the nullable object has a value, the value returned from the delegate; otherwise, the value given.</returns>
        public static TResult IfHasValue<T, TResult>(this T? obj, Func<T, TResult> then, TResult otherwise)
            where T : struct
        {
            return obj.HasValue ? then(obj.Value) : otherwise;
        }

        /// <summary>
        /// Tests whether the nullable object has a value.  If so, invokes the first given delegate and returns its value; otherwise, invokes the second given delegate and returns its value.
        /// </summary>
        /// <typeparam name="T">Underlying type of the nullable object.</typeparam>
        /// <typeparam name="TResult">Type of value to return.</typeparam>
        /// <param name="obj">Extension target object.</param>
        /// <param name="then">Delegate to invoke if the nullable object has a value.</param>
        /// <param name="otherwise">Delegate to invoke if the nullable object does not have a value.</param>
        /// <returns>If the nullable object has a value, the value returned from the first delegate; otherwise, the value returned by the second delegate.</returns>
        public static TResult IfHasValue<T, TResult>(this T? obj, Func<T, TResult> then, Func<TResult> otherwise)
            where T : struct
        {
            return obj.HasValue ? then(obj.Value) : otherwise();
        }

        /// <summary>
        /// Tests whether the nullable object does not have a value.  If it does not, returns the given value; otherwise, returns the value of the nullable object.
        /// </summary>
        /// <typeparam name="T">Underlying type of the nullable object.</typeparam>
        /// <param name="obj">Extension target object.</param>
        /// <param name="then">Value to return if the nullable object does not have a value.</param>
        /// <returns>If the nullable object does not have a value, the given value; otherwise, the value of the nullable object.</returns>
        public static T IfNotHasValue<T>(this T? obj, T then)
            where T : struct
        {
            return obj.HasValue ? obj.Value : then;
        }

        /// <summary>
        /// Tests whether the nullable object does not have a value.  If it does not, invokes the given delegate; otherwise, returns the value of the nullable object.
        /// </summary>
        /// <typeparam name="T">Underlying type of the nullable object.</typeparam>
        /// <param name="obj">Extension target object.</param>
        /// <param name="then">Delegate to invoke if the nullable object does not have a value.</param>
        /// <returns>If the nullable object does not have a value, the value returned by the given delegate; otherwise, the value of the nullable object.</returns>
        public static T IfNotHasValue<T>(this T? obj, Func<T> then)
            where T : struct
        {
            return obj.HasValue ? obj.Value : then();
        }
    }
}
