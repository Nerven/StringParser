using System;
using System.Globalization;

namespace Nerven.StringParser.Core.Build
{
    public sealed class CustomParseStep : ParseStep
    {
        private readonly Predicate<Type> _CanParse;
        private readonly Func<Type, string, CultureInfo, Result> _TryParse;

        private CustomParseStep(Predicate<Type> canParse, Func<Type, string, CultureInfo, Result> tryParse)
        {
            _CanParse = canParse;
            _TryParse = tryParse;
        }

        public static ParseStep Define(Predicate<Type> canParse, Func<Type, string, CultureInfo, Result> tryParse)
        {
            return new CustomParseStep(canParse, tryParse);
        }

        public static ParseStep Define(Type type, Func<string, CultureInfo, Result> tryParse)
        {
            return new CustomParseStep(_type => _type == type, (_type, _s, _cultureInfo) => tryParse(_s, _cultureInfo));
        }

        public static ParseStep Define<TType>(Func<string, CultureInfo, Result> tryParse)
        {
            return Define(typeof(TType), (_s, _cultureInfo) => tryParse(_s, _cultureInfo));
        }

        public static ParseStep Define(Predicate<Type> canParse, Func<Type, string, Result> tryParse)
        {
            return new CustomParseStep(canParse, (_type, _s, _cultureInfo) => tryParse(_type, _s));
        }

        public static ParseStep Define(Type type, Func<string, Result> tryParse)
        {
            return Define(_type => _type == type, (_type, _s, _cultureInfo) => tryParse(_s));
        }

        public static ParseStep Define<TType>(Func<string, Result> tryParse)
        {
            return Define(typeof(TType), tryParse);
        }

        public override bool CanParse(Type type)
        {
            return _CanParse(type);
        }

        public override Result TryParse(Type type, string s, CultureInfo cultureInfo, Func<Type, string, Result> tryParse)
        {
            return _TryParse(type, s, cultureInfo);
        }
    }
}
