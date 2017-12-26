namespace SimpleDatabase.Schemas
{
    public struct TransactionId
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

        public static bool operator ==(TransactionId left, TransactionId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TransactionId left, TransactionId right)
        {
            return !left.Equals(right);
        }
    }
}
