using System;

namespace Nerven.StringParser.Core
{
    public abstract class CustomStringParser : IStringParser
    {
        public abstract bool CanParse(Type type);

        public abstract StringParseResult<object> TryParse(Type type, string s);

        public virtual bool CanParse<T>()
        {
            return CanParse(typeof(T));
        }

        public virtual StringParseResult<T> TryParse<T>(string s)
        {
            return TryParse(typeof(T), s).Cast<T>();
        }

        public virtual object Parse(Type type, string s)
        {
            var _result = TryParse(type, s);
            if (_result.IsValid)
            {
                return _result.Value;
            }

            throw _result.Throw();
        }

        public virtual T Parse<T>(string s)
        {
            var _result = TryParse<T>( s);
            if (_result.IsValid)
            {
                return _result.Value;
            }

            throw _result.Throw();
        }

        public virtual bool TryParse(Type type, string s, out object result)
        {
            var _result = TryParse(type, s);
            if (_result.IsValid)
            {
                result = _result.Value;
                return true;
            }

            result = null;
            return false;
        }

        public virtual bool TryParse<T>(string s, out T result)
        {
            var _result = TryParse<T>(s);
            if (_result.IsValid)
            {
                result = _result.Value;
                return true;
            }

            result = default(T);
            return false;
        }
    }
}
