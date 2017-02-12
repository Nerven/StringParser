using System.Globalization;

namespace Nerven.StringParser.Core.Build
{
    public sealed class StringParserBuilderContext
    {
        public StringParserBuilderContext(CultureInfo cultureInfo)
        {
            CultureInfo = cultureInfo;
        }

        public CultureInfo CultureInfo { get; }
    }
}
