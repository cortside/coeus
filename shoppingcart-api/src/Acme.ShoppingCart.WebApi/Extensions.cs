using System;
using System.Collections.Generic;
using System.Linq;

namespace Acme.ShoppingCart.WebApi {
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions {
        /// <summary>
        /// Attempts to map a transform over a sequence and transforms to null if the transform fails
        /// </summary>
        /// <typeparam name="T">the input type</typeparam>
        /// <typeparam name="K">the output type</typeparam>
        /// <param name="collection">the sequence to map over</param>
        /// <param name="transform">the transform to apply to the map</param>
        /// <returns>the resulting sequence</returns>
        public static IEnumerable<K> SelectOrNull<T, K>(this IEnumerable<T> collection, Func<T, K> transform)
            where K : class {
            return collection.Select(i => {
                try {
                    return transform(i);
                } catch {
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
                    return null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null
                }
            });
        }

        /// <summary>
        /// yields a new collection with the contents of <paramref name="collection"/> and <paramref name="item"/> added at the end
        /// </summary>
        /// <typeparam name="T">the type in the collection</typeparam>
        /// <param name="collection">the collection</param>
        /// <param name="item">the item to include in the new collection</param>
        /// <returns></returns>
        public static IEnumerable<T> With<T>(this IEnumerable<T> collection, T item) {
            foreach (var i in collection)
                yield return i;

            yield return item;
        }
    }
}
