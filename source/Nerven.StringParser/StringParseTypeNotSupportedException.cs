using System;

//// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Nerven.StringParser
{
    public sealed class StringParseTypeNotSupportedException : StringParseException
    {
        public StringParseTypeNotSupportedException(Type parseType)
        {
            ParseType = parseType;
        }

        public Type ParseType { get; }
    }
}
