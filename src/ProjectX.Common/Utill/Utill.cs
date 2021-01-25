using System;

namespace ProjectX.Common
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
    }
}
