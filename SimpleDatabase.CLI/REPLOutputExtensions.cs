using System;
using JetBrains.Annotations;

namespace SimpleDatabase.CLI
{
    public static class REPLOutputExtensions
    {
        public static void WriteLine(this IREPLOutput output, string str)
        {
            output.Write(str);
            output.Write(Environment.NewLine);
        }

        [StringFormatMethod("format")]
        public static void WriteLine(this IREPLOutput output, string format, params object[] args)
        {
            output.WriteLine(string.Format(format, args));
        }
    }
}