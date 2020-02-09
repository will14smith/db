using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SimpleDatabase.Utils
{
    public struct Option<T> : IEquatable<Option<T>>
        where T : class
    {
        public bool HasValue { get; }
        public T? Value { get; }

        public Option(T? value) { HasValue = true; Value = value; }

        public Option<TOut> Select<TOut>(Func<T, TOut> fn) where TOut : class
        {
            return HasValue ? new Option<TOut>(fn(Value!)) : new Option<TOut>();
        }
        public TOut Map<TOut>([InstantHandle] Func<T, TOut> some, [InstantHandle] Func<TOut> none)
        {
            return HasValue ? some(Value!) : none();
        }
        
        public static implicit operator Option<T>(Option.NoneOption _)
        {
            return new Option<T>();
        }

        public override bool Equals(object obj)
        {
            return obj is Option<T> other && Equals(other);
        }

        public bool Equals(Option<T> other)
        {
            if (HasValue != other.HasValue) return false;
            if (!HasValue) return true;
            
            return EqualityComparer<T>.Default.Equals(Value!, other.Value!);
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = HasValue.GetHashCode() * 397;
                if (HasValue)
                {
                    hashCode = hashCode ^ EqualityComparer<T>.Default.GetHashCode(Value!);
                }
                return hashCode;
            }
        }

        public static bool operator ==(Option<T> left, Option<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Option<T> left, Option<T> right)
        {
            return !left.Equals(right);
        }
    }

    public static class Option
    {
        public static Option<T> Some<T>(T value)
            where T : class
        {
            return new Option<T>(value);
        }

        public static Option<T> None<T>()
            where T : class
        {
            return new Option<T>();
        }
        
        public static NoneOption None()
        {
            return new NoneOption();
        }

        public class NoneOption { }
    }

}
