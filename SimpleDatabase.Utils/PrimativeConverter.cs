using System;
using System.Text;

namespace SimpleDatabase.Utils
{
    public static class PrimativeConverter
    {
        public static T Read<T>(this Span<byte> span)
            where T : struct
        {
            if (!BitConverter.IsLittleEndian)
            {
                throw new NotImplementedException();
            }

            return span.NonPortableCast<byte, T>()[0];
        }
        public static void Write<T>(this Span<byte> span, T value)
            where T : struct
        {
            if (!BitConverter.IsLittleEndian)
            {
                throw new NotImplementedException();
            }

            span.NonPortableCast<byte, T>()[0] = value;
        }

        public static string ReadString(this Span<byte> span)
        {
            var len = 0;
            while (len < span.Length && span[len] != 0)
            {
                len++;
            }

            // TODO handle UTF8
            var encoding = Encoding.ASCII;

            // TODO make this use Spans better
            var buffer = span.Slice(0, len).ToArray();
            return encoding.GetString(buffer);
        }
        public static void WriteString(this Span<byte> span, string value)
        {
            // TODO handle UTF8
            var encoding = Encoding.ASCII;

            if (encoding.GetByteCount(value) > span.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"Length must be < {span.Length}");
            }

            // TODO make this use Spans better
            var bytes = encoding.GetBytes(value);
            bytes.CopyTo(span);

            if (bytes.Length < span.Length)
            {
                span[bytes.Length] = 0;
            }
        }
    }
}
