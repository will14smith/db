using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleDatabase.Utils
{
    public class DictionaryComparer<TKey, TValue> : IEqualityComparer<IDictionary<TKey, TValue>>
    {
        private readonly IEqualityComparer<TValue> _valueComparer;

        public DictionaryComparer(IEqualityComparer<TValue>? valueComparer = null)
        {
            _valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;
        }

        public bool Equals(IDictionary<TKey, TValue> x, IDictionary<TKey, TValue> y)
        {
            if (x.Count != y.Count) return false;

            if (x.Keys.Except(y.Keys).Any()) return false;
            if (y.Keys.Except(x.Keys).Any()) return false;

            return x.All(pair => _valueComparer.Equals(pair.Value, y[pair.Key]));
        }

        public int GetHashCode(IDictionary<TKey, TValue> obj)
        {
            throw new NotImplementedException();
        }
    }

}
