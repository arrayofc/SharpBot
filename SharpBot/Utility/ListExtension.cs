using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SharpBot.Utility {
    public static class ListExtension {

        /// <summary>
        /// Chops a list into multiple pieces for pagination.
        /// </summary>
        /// <param name="list">The list to chop.</param>
        /// <param name="size">The desired size for each part.</param>
        /// <typeparam name="T">the type of the list</typeparam>
        /// <returns>The chopped parts of the list.</returns>
        public static List<List<T>> ChopList<T>(this ImmutableList<T> list, int size) {
            var parts = new List<List<T>>();
            var listSize = list.Count;

            for (var i = 0; i < listSize; i += size) {
                parts.Add(list.ToList().GetRange(i, Math.Min(size, listSize - i)));
            }

            return parts;
        }
    }
}