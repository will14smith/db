using System;
using System.Collections.Generic;

namespace SimpleDatabase.Options
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
        {
            return opt.Map(x => x, func);
        }

        public static Option<T> Flatten<T>(this Option<Option<T>> opt)
        {
            return opt.Select(x => x.Value);
        }
        public static Option<TOut> Flatten<TIn, TOut>(this Option<TIn> opt, Func<TIn, Option<TOut>> some)
        {
            return opt.Select(some).Select(x => x.Value);
        }

        public static Option<TCast> As<TValue, TCast>(this Option<TValue> opt)
            where TCast : class
        {
            return opt.Flatten(x => (x as TCast).ToOption());
        }

        public static Option<TValue> TryGet<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key)
        {
            return dict.TryGetValue(key, out var value)
                ? Option.Some(value)
                : Option.None<TValue>();
        }
    }

}
