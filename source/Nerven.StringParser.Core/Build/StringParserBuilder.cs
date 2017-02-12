using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

//// ReSharper disable UnusedAutoPropertyAccessor.Global
//// ReSharper disable MemberCanBePrivate.Global
namespace Nerven.StringParser.Core.Build
{
    public sealed class StringParserBuilder
    {
        public StringParserBuilder()
        {
            PreSteps = new List<Func<StringParserBuilderContext, ParseStep>>();
            Steps = new List<Func<StringParserBuilderContext, ParseStep>>();
            PostSteps = new List<Func<StringParserBuilderContext, ParseStep>>();
        }

        public CultureInfo CultureInfo { get; set; }

        public List<Func<StringParserBuilderContext, ParseStep>> PreSteps { get; set; }

        public List<Func<StringParserBuilderContext, ParseStep>> Steps { get; set; }

        public List<Func<StringParserBuilderContext, ParseStep>> PostSteps { get; set; }

        public IStringParser Build()
        {
            var _context = new StringParserBuilderContext(CultureInfo);
            var _steps = (PreSteps ?? Enumerable.Empty<Func<StringParserBuilderContext, ParseStep>>())
                .Concat(Steps ?? Enumerable.Empty<Func<StringParserBuilderContext, ParseStep>>())
                .Concat(PostSteps ?? Enumerable.Empty<Func<StringParserBuilderContext, ParseStep>>())
                .ToList()
                .Select(_stepCreator => _stepCreator(_context))
                .ToList();
            return new _BuildableStringParser(CultureInfo, _steps);
        }

        private sealed class _BuildableStringParser : CustomStringParser
        {
            private readonly CultureInfo _CultureInfo;
            private readonly List<ParseStep> _ParseSteps;
            private readonly ConcurrentDictionary<Type, ParseStep[]> _TypeParseSteps;

            public _BuildableStringParser(CultureInfo cultureInfo, List<ParseStep> parseSteps)
            {
                _CultureInfo = cultureInfo;
                _ParseSteps = parseSteps;
                _TypeParseSteps = new ConcurrentDictionary<Type, ParseStep[]>();
            }

            public override bool CanParse(Type type)
            {
                return _GetParseSteps(type).Length != 0;
            }

            public override StringParseResult<object> TryParse(Type type, string s)
            {
                if (type == typeof(object))
                {
                    return StringParseResult.UnsupportedType(type, s);
                }

                var _result = _CreateParseContext()(type, s);

                // Ensure relevant result
                return _result.ToStringParseResult(type, s);
            }

            private Func<Type, string, ParseStep.Result> _CreateParseContext()
            {
                var _stack = new HashSet<ParseStep>();
                Func<Type, string, ParseStep.Result> _tryParse = null;
                _tryParse = (_type, _s) =>
                    {
                        var _parseSteps = _GetParseSteps(_type);
                        if (_parseSteps.Length == 0)
                        {
                            return StringParseResult.UnsupportedType(_type, _s);
                        }

                        foreach (var _parseStep in _parseSteps)
                        {
                            if (_stack.Add(_parseStep))
                            {
                                return _parseStep.TryParse(_type, _s, _CultureInfo, _tryParse);
                            }
                        }

                        return StringParseResult.InvalidString(_type, _s);
                    };

                return _tryParse;
            }

            private ParseStep[] _GetParseSteps(Type type)
            {
                return _TypeParseSteps.GetOrAdd(type, _type =>
                    {
                        return _ParseSteps.Where(_ps => _ps.CanParse(_type)).ToArray();
                    });
            }
        }
    }
}
