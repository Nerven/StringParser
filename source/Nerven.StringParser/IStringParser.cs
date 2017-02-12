using System;

namespace Nerven.StringParser
{
    public interface IStringParser
    {
        bool CanParse(Type type);

        bool CanParse<T>();

        object Parse(Type type, string s);

        T Parse<T>(string s);

        bool TryParse(Type type, string s, out object result);

        bool TryParse<T>(string s, out T result);

        StringParseResult<object> TryParse(Type type, string s);

        StringParseResult<T> TryParse<T>(string s);
    }
}
