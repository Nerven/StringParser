using System;

namespace Nerven.StringParser
{
    public struct StringParseResult<T>
    {
        internal StringParseResult(Type parseType, bool? isTypeSupported, bool? isStringValid, string s, T value)
        {
            ParseType = parseType;
            IsTypeSupported = isTypeSupported;
            IsStringValid = isStringValid;
            String = s;
            Value = value;
        }

        public Type ParseType { get; }

        public bool? IsTypeSupported { get; }

        public bool? IsStringValid { get; }

        public string String { get; }

        public T Value { get; }

        public bool IsValid => IsStringValid.GetValueOrDefault() && (!IsTypeSupported.HasValue || IsTypeSupported.Value);

        public StringParseResult<TNew> ConvertValid<TNew>(Func<T, TNew> convert)
        {
            if (convert == null)
            {
                throw new ArgumentNullException(nameof(convert));
            }

            if (!IsValid)
            {
                return new StringParseResult<TNew>(ParseType, IsTypeSupported, IsStringValid, String, default(TNew));
            }

            return new StringParseResult<TNew>(ParseType, null, IsStringValid, String, convert(Value));
        }

        public StringParseResult<TNew> Cast<TNew>()
            where TNew : T
        {
            return ConvertValid(_value => (TNew)_value);
        }

        public StringParseException Throw()
        {
            if (IsValid)
            {
                throw new InvalidOperationException($"{nameof(StringParseResult)}<>.{nameof(Throw)} may only be used on invalid results.");
            }

            if (!IsTypeSupported.GetValueOrDefault(true))
            {
                throw new StringParseTypeNotSupportedException(ParseType);
            }

            if (!IsStringValid.GetValueOrDefault(true))
            {
                throw new StringParseInvalidStringException(ParseType, String);
            }

            throw new UnexpectedStringParseErrorException();
        }
    }
}
