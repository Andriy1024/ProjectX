using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectX.Core
{
    public static class Utill
    {
        public static void ThrowIfNull<T>(T item, string message)
            where T : class
        {
            if (item == null)
                throw new ArgumentNullException(message);
        }

        public static void ThrowIfNullOrEmpty(string text, string message)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException(message);
        }

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
