using System;using SimpleDatabase.Utils;namespace SimpleDatabase.Core
{
    public class Row
    {
        public const int IdSize = sizeof(int);
        public const int UsernameSize = 32;
        public const int EmailSize = 255;

        public const int IdOffset = 0;
        public const int UsernameOffset = IdOffset + IdSize;
        public const int EmailOffset = UsernameOffset + UsernameSize;

        public const int RowSize = IdSize + UsernameSize + EmailSize;

        public int Id { get; }
        public string Username { get; }
        public string Email { get; }

        public Row(int id, string username, string email)
        {
            if (username.Length > UsernameSize) throw new ArgumentOutOfRangeException(nameof(username), $"Length must be <= {UsernameSize}");
            if (email.Length > EmailSize) throw new ArgumentOutOfRangeException(nameof(email), $"Length must be <= {EmailSize}");

            Id = id;
            Username = username;
            Email = email;
        }

        public override string ToString()
        {
            return $"({Id}, {Username}, {Email})";
        }

        public void Serialize(Span<byte> dest)
        {
            dest.Write(Id);
            dest.Slice(UsernameOffset, UsernameSize).WriteString(Username);
            dest.Slice(EmailOffset, EmailSize).WriteString(Email);
        }

        public static Row Deserialize(Span<byte> source)
        {
            var id = source.Read<int>();
            var username = source.Slice(UsernameOffset, UsernameSize).ReadString();
            var email = source.Slice(EmailOffset, EmailSize).ReadString();

            return new Row(id, username, email);
        }

        public object GetColumn(int columnIndex)
        {
            switch (columnIndex)
            {
                case 0: return Id;
                case 1: return Username;
                case 2: return Email;

                default: throw new ArgumentOutOfRangeException(nameof(columnIndex));
            }
        }
    }
}
