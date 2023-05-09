using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SimpleDatabase.Utils
{
    public static class PrimativeConverter
    {
        public static T Read<T>(this Span<byte> span) where T : struct => Read<T>((ReadOnlySpan<byte>)span);
        public static T Read<T>(this ReadOnlySpan<byte> span)
            where T : struct
        {
            if (!BitConverter.IsLittleEndian)
            {
                throw new NotImplementedException();
            }

            return MemoryMarshal.Cast<byte, T>(span)[0];
        }
        public static void Write<T>(this Span<byte> span, T value)
            where T : struct
        {
            if (!BitConverter.IsLittleEndian)
            {
                throw new NotImplementedException();
            }

            MemoryMarshal.Cast<byte, T>(span)[0] = value;
        }

        public static string ReadCString(this Span<byte> span) => ReadCString((ReadOnlySpan<byte>)span);
        public static string ReadCString(this ReadOnlySpan<byte> span)
        {
            var len = 0;
            while (len < span.Length && span[len] != 0)
            {
                len++;
            }

            // TODO handle UTF8
            var encoding = Encoding.ASCII;
            
            return encoding.GetString(span[..len]);
        }
        public static void WriteCString(this Span<byte> span, string value)
        {
            // TODO handle UTF8
            var encoding = Encoding.ASCII;

            if (encoding.GetByteCount(value) > span.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"Length must be < {span.Length}");
            }

            var written = encoding.GetBytes(value, span);
            if (written < span.Length)
            {
                span[written] = 0;
            }
        }

        public static string ReadU8LengthString(this Span<byte> span) => ReadU8LengthString((ReadOnlySpan<byte>)span);
        public static string ReadU8LengthString(this ReadOnlySpan<byte> span)
        {
            // TODO handle UTF8
            var encoding = Encoding.ASCII;

            var length = span[0];
            var str = span.Slice(1, length);
            return encoding.GetString(str);
        }


        public static int WriteU8LengthString(this Span<byte> span, string value)
        {
            var encoding = Encoding.ASCII;

            var length = encoding.GetByteCount(value);
            if (length > byte.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"Length must be <= {byte.MaxValue}");
            }
            if (length+1 > span.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"Length must be < {span.Length-1}");
            }

            span[0] = (byte)length;
            encoding.GetBytes(value, span[1..]);

            return length + 1;
        }
    }
}
