using System;
using System.Globalization;

namespace Nerven.StringParser.Core.Build
{
    public abstract class ParseStep
    {
        public static implicit operator Func<StringParserBuilderContext, ParseStep>(ParseStep deferred)
        {
            return _ => deferred;
        }

        public abstract bool CanParse(Type type);

        public abstract Result TryParse(
            Type type,
            string s,
            CultureInfo cultureInfo,
            Func<Type, string, Result> tryParse);

        public IStringParser CreateStringParser(CultureInfo cultureInfo)
        {
            return new _ParseStepStringParser(this, cultureInfo);
        }

        public static Result Valid(object value)
        {
            return new Result(value, null, isTypeSupported: true, isStringValid: true);
        }

        public static Result UnsupportedType()
        {
            return new Result(null, null, isTypeSupported: false, isStringValid: null);
        }

        public static Result InvalidString()
        {
            return new Result(null, null, isTypeSupported: true, isStringValid: false);
        }

        public struct Result
        {
            public Result(object value, StringParseResult<object>? underlyingResult, bool? isTypeSupported, bool? isStringValid)
            {
                Value = value;
                UnderlyingResult = underlyingResult;
                IsTypeSupported = isTypeSupported;
                IsStringValid = isStringValid;
            }

            public object Value { get; }

            public StringParseResult<object>? UnderlyingResult { get; }

            public bool? IsTypeSupported { get; }

            public bool? IsStringValid { get; }

            public static implicit operator Result(StringParseResult<object> result)
            {
                return new Result(result.Value, result, result.IsTypeSupported, result.IsStringValid);
            }

            public StringParseResult<object> ToStringParseResult(Type type, string s)
            {
                if (UnderlyingResult.HasValue)
                {
                    return UnderlyingResult.Value;
                }

                if (IsTypeSupported.GetValueOrDefault() && IsStringValid.GetValueOrDefault())
                {
                    return StringParseResult.Valid(type, s, Value);
                }

                if (!IsTypeSupported.GetValueOrDefault(true))
                {
                    return StringParseResult.UnsupportedType(type, s);
                }

                if (!IsStringValid.GetValueOrDefault(true))
                {
                    return StringParseResult.InvalidString(type, s);
                }

                throw new UnexpectedStringParseErrorException();
            }
        }

        private sealed class _ParseStepStringParser : CustomStringParser
        {
            private readonly ParseStep _ParseStep;
            private readonly CultureInfo _CultureInfo;

            public _ParseStepStringParser(ParseStep parseStep, CultureInfo cultureInfo)
            {
                _ParseStep = parseStep;
                _CultureInfo = cultureInfo;
            }

            public override bool CanParse(Type type)
            {
                return _ParseStep.CanParse(type);
            }

            public override StringParseResult<object> TryParse(Type type, string s)
            {
                var _result = _ParseStep.TryParse(type, s, _CultureInfo, _FailParse);
                return _result.ToStringParseResult(type, s);
            }

            private static Result _FailParse(Type type, string s)
            {
                return StringParseResult.UnsupportedType(type, s);
            }
        }
    }
}
