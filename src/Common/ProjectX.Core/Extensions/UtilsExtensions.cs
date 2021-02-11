using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectX.Core
{
    public static class UtilsExtensions
    {
        public static bool IsOneOf<T>(this T target, params T[] items)
            => items.Contains(target);

        public static bool IsOneOf<T>(this T target, IEnumerable<T> items)
            => items.Contains(target);

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> target)
            => target == null || !target.Any();

        public static bool IsNullOrEmpty<T>(this T[] target)
            => target == null || target.Length == 0;
    }
}
