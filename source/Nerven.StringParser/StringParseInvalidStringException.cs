using System;

//// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Nerven.StringParser
{
    public sealed class StringParseInvalidStringException : StringParseException
    {
        public StringParseInvalidStringException(Type parseType, string s)
        {
            ParseType = parseType;
            String = s;
        }

        public Type ParseType { get; }

        public string String { get; }
    }
}
