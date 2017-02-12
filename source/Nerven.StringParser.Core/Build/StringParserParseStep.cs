using System;
using System.Globalization;

namespace Nerven.StringParser.Core.Build
{
    public sealed class StringParserParseStep : ParseStep
    {
        private readonly IStringParser _UnderlyingStringParser;

        private StringParserParseStep(IStringParser underlyingStringParser)
        {
            _UnderlyingStringParser = underlyingStringParser;
        }

        public static StringParserParseStep Create(IStringParser stringParser)
        {
            return new StringParserParseStep(stringParser);
        }
        
        public override bool CanParse(Type type)
        {
            return _UnderlyingStringParser.CanParse(type);
        }

        public override Result TryParse(Type type, string s, CultureInfo cultureInfo, Func<Type, string, Result> tryParse)
        {
            return _UnderlyingStringParser.TryParse(type, s);
        }
    }
}
