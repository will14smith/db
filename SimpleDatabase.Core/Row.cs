using System;
using System.Text;

namespace SimpleDatabase.Core
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

        public void Serialize(byte[] destination, int offset)
        {
            var idBytes = BitConverter.GetBytes(Id);

            Array.Copy(idBytes, 0, destination, offset + IdOffset, IdSize);
            WriteString(Username, destination, offset + UsernameOffset, UsernameSize);
            WriteString(Email, destination, offset + EmailOffset, EmailSize);
        }

        public static Row Deserialize(byte[] source, int offset)
        {
            var id = BitConverter.ToInt32(source, offset + IdOffset);
            var username = ReadString(source, offset + UsernameOffset, UsernameSize);
            var email = ReadString(source, offset + EmailOffset, EmailSize);

            return new Row(id, username, email);
        }

        private static void WriteString(string source, byte[] destination, int offset, int size)
        {
            // TODO handle UTF8
            var encoding = Encoding.ASCII;

            if (encoding.GetByteCount(source) > size)
            {
                throw new ArgumentOutOfRangeException(nameof(source), $"Length must be < {size}");
            }

            var count = encoding.GetBytes(source, 0, source.Length, destination, offset);
            if (count < size)
            {
                destination[offset + count] = 0;
            }
        }
        private static string ReadString(byte[] source, int offset, int size)
        {
            var len = 0;
            while (len < size && source[offset + len] != 0)
            {
                len++;
            }

            // TODO handle UTF8
            var encoding = Encoding.ASCII;

            return encoding.GetString(source, offset, len);
        }
    }
}
