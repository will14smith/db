using System;
using System.Collections.Generic;

namespace SimpleDatabase.Utils
{
    public static class OptionExtensions
    {
        public static Option<T> ToOption<T>(this T value)
            where T : class
        {
            return value != default(T) 
                ? Option.Some(value) 
                : Option.None<T>();
        }


        public static T OrElse<T>(this Option<T> opt, Func<T> func)
            where T : class
        {
            return opt.Map(x => x, func);
        }
        
        public static Option<TOut> Flatten<TIn, TOut>(this Option<TIn> opt, Func<TIn, Option<TOut>> some) 
            where TIn : class
            where TOut : class
        {
            return opt.HasValue ? some(opt.Value!) : Option.None<TOut>();
        }

        public static Option<TCast> As<TValue, TCast>(this Option<TValue> opt)
            where TValue : class
            where TCast : class
        {
            return opt.Flatten(x =>
            {
                if(x is TCast value) return new Option<TCast>(value);
                return Option.None<TCast>();
            });
        }

        public static Option<TValue> TryGet<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key)
            where TValue : class
        {
            return dict.TryGetValue(key, out var value)
                ? Option.Some(value)
                : Option.None<TValue>();
        }
    }

}
