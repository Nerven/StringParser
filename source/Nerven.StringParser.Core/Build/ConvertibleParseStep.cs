using System;
using System.Globalization;

namespace Nerven.StringParser.Core.Build
{
    public static class ConvertibleParseStep
    {
        public static Func<StringParserBuilderContext, ParseStep> Default => _ => Create(_.CultureInfo);

        public static ParseStep Create(CultureInfo cultureInfo) => StringParserParseStep.Create(ConvertibleStringParser.Create(cultureInfo));
    }
}
