using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProjectX.Core
{
    public static class ValidationUtility
    {
        public static void ThrowIfInvalid<T>(this T obj)
        {
            if (obj is null)
                throw new ArgumentNullException($"{typeof(T).Name}");

            var ctx = new ValidationContext(obj);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(obj, ctx, results, true))
            {
                throw new ValidationException($"{typeof(T).Name}: {string.Join(',', results.Select(r => r.ErrorMessage))}");
            }
        }
    }
}
