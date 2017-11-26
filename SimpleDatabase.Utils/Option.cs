using System;
using JetBrains.Annotations;

namespace SimpleDatabase.Utils
{
    public struct Option<T>
    {
        public bool HasValue { get; }
        public T Value { get; }

        public Option(T value) { HasValue = true; Value = value; }

        public Option<TOut> Select<TOut>(Func<T, TOut> fn)
        {
            return HasValue ? new Option<TOut>(fn(Value)) : new Option<TOut>();
        }
        public TOut Map<TOut>([InstantHandle] Func<T, TOut> some, [InstantHandle] Func<TOut> none)
        {
            return HasValue ? some(Value) : none();
        }
    }

    public static class Option
    {
        public static Option<T> Some<T>(T value)
        {
            return new Option<T>(value);
        }

        public static Option<T> None<T>()
        {
            return new Option<T>();
        }
    }

}
