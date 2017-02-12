using System;
using System.Globalization;

namespace Nerven.StringParser.Core.Build
{
    public sealed class NullableParseStep : ParseStep
    {
        private NullableParseStep()
        {
        }

        public static NullableParseStep Default { get; } = new NullableParseStep();

        public override bool CanParse(Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        public override Result TryParse(
            Type type,
            string s,
            CultureInfo cultureInfo,
            Func<Type, string, Result> tryParse)
        {
            var _underlyingType = Nullable.GetUnderlyingType(type);
            if (_underlyingType == null)
            {
                return UnsupportedType();
            }

            if (string.IsNullOrEmpty(s))
            {
                return Valid(null);
            }

            return tryParse(_underlyingType, s);
        }
    }
}
