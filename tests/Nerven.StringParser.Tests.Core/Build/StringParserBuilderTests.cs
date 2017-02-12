using System;
using System.Globalization;
using Nerven.StringParser.Core.Build;
using Xunit;

namespace Nerven.StringParser.Tests.Core.Build
{
    public class StringParserBuilderTests
    {
        [Fact]
        public void CommonCases()
        {
            var _builder = new StringParserBuilder
                {
                    PreSteps =
                        {
                            NullableParseStep.Default,
                        },
                    Steps =
                        {
                            EnumParseStep.Ordinal,
                            CustomParseStep.Define<ushort>(_s =>
                                {
                                    ushort _value;
                                    return ushort.TryParse(_s, out _value)
                                        ? ParseStep.Valid((ushort)(_value + 1))
                                        : ParseStep.InvalidString();
                                }),
                        },
                    PostSteps =
                        {
                            ConvertibleParseStep.Default,
                        },
                };

            var _stringParser = _builder.Build();

            Assert.Equal(true, _stringParser.Parse<bool?>("True"));
            Assert.Equal(false, _stringParser.Parse<bool>("False"));
            Assert.Equal(null, _stringParser.Parse<bool?>(null));
            Assert.Equal("Test", _stringParser.Parse<string>("Test"));
            Assert.Equal(22, _stringParser.Parse<int>("22"));
            Assert.Equal('@', _stringParser.Parse<char>("@"));
            Assert.Equal(null, _stringParser.Parse<int?>(string.Empty));
            Assert.Equal(UriKind.RelativeOrAbsolute, _stringParser.Parse<UriKind>("RelativeOrAbsolute"));
            Assert.Equal(UriKind.Absolute, _stringParser.Parse<UriKind?>("Absolute"));
            Assert.Equal(null, _stringParser.Parse<UriKind?>(string.Empty));
            Assert.Equal(NumberStyles.AllowExponent | NumberStyles.AllowParentheses, _stringParser.Parse<NumberStyles?>("AllowExponent  , AllowParentheses "));
            Assert.Equal((ushort?)1, _stringParser.Parse<ushort?>("0"));

            Assert.NotEqual(null, _stringParser.Parse<UriKind?>("Absolute"));

            Assert.True(_stringParser.TryParse<bool?>("True").IsValid);
            Assert.True(_stringParser.TryParse<UriKind?>(string.Empty).IsValid);
            Assert.True(_stringParser.TryParse<int?>(string.Empty).IsValid);
            Assert.True(_stringParser.TryParse<string>("Nope!").IsValid);
            Assert.True(_stringParser.TryParse<char>("!").IsValid);
            Assert.True(_stringParser.TryParse<NumberStyles>("AllowExponent  , AllowParentheses ").IsValid);

            Assert.False(_stringParser.TryParse<UriKind>("Absolute!").IsValid);
            Assert.False(_stringParser.TryParse<UriKind>(string.Empty).IsValid);
            Assert.False(_stringParser.TryParse<int>(string.Empty).IsValid);
            Assert.False(_stringParser.TryParse<int?>("Nope!").IsValid);
            Assert.False(_stringParser.TryParse<int>("Nope!").IsValid);
            Assert.False(_stringParser.TryParse<int>("Nope!").IsValid);
            Assert.False(_stringParser.TryParse<char>("!!").IsValid);
            Assert.False(_stringParser.TryParse<Type>(GetType().FullName).IsValid);
            Assert.False(_stringParser.TryParse<Type>("System.String").IsValid);
            Assert.False(_stringParser.TryParse<Type>("!!").IsValid);
            Assert.False(_stringParser.TryParse<object>("Test").IsValid);
        }

        [Fact]
        public void CommonCasesDifferentCultures()
        {
            var _builder = new StringParserBuilder
                {
                    PreSteps =
                    {
                        NullableParseStep.Default,
                    },
                    Steps =
                    {
                        EnumParseStep.Ordinal,
                    },
                    PostSteps =
                    {
                        ConvertibleParseStep.Default,
                    },
                };

            _builder.CultureInfo = CultureInfo.GetCultureInfo("en-US");
            var _stringParserEnUs = _builder.Build();
            _builder.CultureInfo = CultureInfo.GetCultureInfo("sv-SE");
            var _stringParserSvSe = _builder.Build();
            _builder.CultureInfo = CultureInfo.GetCultureInfo("ja-JP");
            var _stringParserJaJp = _builder.Build();
            _builder.CultureInfo = CultureInfo.GetCultureInfo("it");
            var _stringParserIt = _builder.Build();

            Assert.Equal(22, _stringParserEnUs.Parse<int>("22"));
            Assert.Equal(22, _stringParserSvSe.Parse<int>("22"));
            Assert.Equal(22, _stringParserJaJp.Parse<int>("22"));
            Assert.Equal(22, _stringParserIt.Parse<int>("22"));

            Assert.Equal(22D, _stringParserEnUs.Parse<double>("22.000"));
            Assert.Equal(22D, _stringParserSvSe.Parse<double>("22,000"));
            Assert.Equal(22D, _stringParserJaJp.Parse<double>("22.000"));
            Assert.Equal(22D, _stringParserIt.Parse<double>("22,000"));

            Assert.Equal(22000D, _stringParserEnUs.Parse<double>("22,000"));
            Assert.Equal(22000D, _stringParserSvSe.Parse<double>("22 000"));
            Assert.Equal(22000D, _stringParserJaJp.Parse<double>("22,000"));
            Assert.Equal(22000D, _stringParserIt.Parse<double>("22.000"));

            Assert.Equal(true, _stringParserEnUs.Parse<bool>("True"));
            Assert.Equal(true, _stringParserSvSe.Parse<bool>("True"));
            Assert.Equal(true, _stringParserJaJp.Parse<bool>("True"));
            Assert.Equal(true, _stringParserIt.Parse<bool>("True"));
        }
    }
}
