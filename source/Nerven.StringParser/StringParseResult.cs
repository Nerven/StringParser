using System;

namespace Nerven.StringParser
{
    public static class StringParseResult
    {
        public static StringParseResult<object> Valid(Type type, string s, object value) => new StringParseResult<object>(type, true, true, s, value);

        public static StringParseResult<object> UnsupportedType(Type type, string s) => new StringParseResult<object>(type, false, null, s, null);

        public static StringParseResult<object> InvalidString(Type type, string s) => new StringParseResult<object>(type, true, false, s, null);

        public static StringParseResult<T> Valid<T>(Type type, string s, T value) => new StringParseResult<T>(type, true, true, s, value);

        public static StringParseResult<T> UnsupportedType<T>(Type type, string s) => new StringParseResult<T>(type, false, null, s, default(T));

        public static StringParseResult<T> InvalidString<T>(Type type, string s) => new StringParseResult<T>(type, true, false, s, default(T));
    }
}
