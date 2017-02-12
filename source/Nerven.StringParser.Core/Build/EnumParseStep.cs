using System;
using System.Globalization;
using System.Reflection;
using EnumsNET.NonGeneric;

namespace Nerven.StringParser.Core.Build
{
    public sealed class EnumParseStep : ParseStep
    {
        private readonly bool _CaseSensitive;

        private EnumParseStep(bool caseSensitive)
        {
            _CaseSensitive = caseSensitive;
        }

        public static ParseStep Ordinal { get; } = new EnumParseStep(true);

        public static ParseStep OrdinalIgnoreCase { get; } = new EnumParseStep(false);

        public override bool CanParse(Type type)
        {
            return type.GetTypeInfo().IsEnum;
        }

        public override Result TryParse(Type type, string s, CultureInfo cultureInfo, Func<Type, string, Result> tryParse)
        {
            object _result;
            if (NonGenericEnums.TryParse(type, s, !_CaseSensitive, out _result))
            {
                return Valid(_result);
            }
            
            return InvalidString();
        }
    }
}
