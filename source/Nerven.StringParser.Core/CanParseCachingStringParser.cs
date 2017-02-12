using System;
using System.Collections.Concurrent;

namespace Nerven.StringParser.Core
{
    public sealed class CanParseCachingStringParser : CustomStringParser
    {
        private readonly IStringParser _UnderlyingStringParser;
        private readonly ConcurrentDictionary<Type, bool> _TypeCache;

        private CanParseCachingStringParser(IStringParser underlyingStringParser, ConcurrentDictionary<Type, bool> typeCache)
        {
            _UnderlyingStringParser = underlyingStringParser;
            _TypeCache = typeCache;
        }

        public static CanParseCachingStringParser Create(IStringParser underlyingStringParser)
        {
            return new CanParseCachingStringParser(underlyingStringParser, new ConcurrentDictionary<Type, bool>());
        }

        public static CanParseCachingStringParser Create(IStringParser underlyingStringParser, ConcurrentDictionary<Type, bool> typeCache)
        {
            return new CanParseCachingStringParser(underlyingStringParser, typeCache);
        }

        public override bool CanParse(Type type)
        {
            return _TypeCache.GetOrAdd(type, _type => _UnderlyingStringParser.CanParse(_type));
        }

        public override StringParseResult<object> TryParse(Type type, string s)
        {
            bool _canParse;
            if (_TypeCache.TryGetValue(type, out _canParse))
            {
                if (!_canParse)
                {
                    return StringParseResult.UnsupportedType(type, s);
                }

                return _UnderlyingStringParser.TryParse(type, s);
            }

            var _result = _UnderlyingStringParser.TryParse(type, s);
            _TypeCache.TryAdd(type, _result.IsTypeSupported.GetValueOrDefault());
            return _result;
        }
    }
}
