using System;
using System.Collections.Generic;

namespace SimpleDatabase.Utils
{
    public static class NullableExtensions
    {
        public static TOut? Select<TIn, TOut>(this TIn? value, Func<TIn, TOut> selector)
            where TIn : struct
            where TOut : struct
        {
            return value.HasValue ? selector(value.Value) : default(TOut?);
        }  
        
        public static TOut Select<TIn, TOut>(this TIn value, Func<TIn, TOut> selector)
            where TIn : class?
            where TOut : class?
        {
            return value != null ? selector(value) : null;
        }    
        
        public static T OrElse<T>(this T? opt, Func<T> func)
            where T : struct
        {
            return opt ?? func();
        }
        
        public static TValue? TryGet<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key)
            where TValue : struct
        {
            return dict.TryGetValue(key, out var value) ? value : (TValue?) null;
        }
    }
}