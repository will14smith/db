﻿using System;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Schemas
{
    public struct TransactionId : IComparable<TransactionId>
    {
        public ulong Id { get; }

        public TransactionId(ulong id)
        {
            Id = id;
        }

        public bool Equals(TransactionId other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is TransactionId id && Equals(id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public int CompareTo(TransactionId other)
        {
            return Id.CompareTo(other.Id);
        }

        public static bool operator ==(TransactionId left, TransactionId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TransactionId left, TransactionId right)
        {
            return !left.Equals(right);
        }

        public static bool operator <(TransactionId left, TransactionId right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(TransactionId left, TransactionId right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(TransactionId left, TransactionId right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(TransactionId left, TransactionId right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static TransactionId? Some(ulong id)
        {
            return new TransactionId(id);
        }
        public static TransactionId? None()
        {
            return null;
        }
    }
}
