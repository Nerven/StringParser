using System;
using System.Collections.Concurrent;
using System.Globalization;

namespace Nerven.StringParser.Core
{
    public sealed class ConvertibleStringParser : CustomStringParser
    {
        private static readonly ConcurrentDictionary<Type, bool> _TypeCache = new ConcurrentDictionary<Type, bool>();

        private readonly CultureInfo _CultureInfo;

        private ConvertibleStringParser(CultureInfo cultureInfo)
        {
            _CultureInfo = cultureInfo;
        }

        public static IStringParser InvariantCulture { get; } = CanParseCachingStringParser.Create(
            new ConvertibleStringParser(CultureInfo.InvariantCulture),
            _TypeCache);

        public static IStringParser Create(CultureInfo cultureInfo) => CanParseCachingStringParser.Create(
            new ConvertibleStringParser(cultureInfo),
            _TypeCache);

        public override bool CanParse(Type type)
        {
            try
            {
                //// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                Convert.ChangeType(string.Empty, type, _CultureInfo);
                return true;
            }
            catch (InvalidCastException)
            {
                return false;
            }
            catch (FormatException)
            {
                return true;
            }
            catch (OverflowException)
            {
                return true;
            }
        }

        public override StringParseResult<object> TryParse(Type type, string s)
        {
            try
            {
                return StringParseResult.Valid(type, s, Convert.ChangeType(s, type, _CultureInfo));
            }
            catch (InvalidCastException)
            {
                return StringParseResult.UnsupportedType(type, s);
            }
            catch (FormatException)
            {
                return StringParseResult.InvalidString(type, s);
            }
            catch (OverflowException)
            {
                return StringParseResult.InvalidString(type, s);
            }
        }
    }
}
