Extensible C# string parser with decent defaults, .NET primitives support, enum support, nullable support.

````c#
using Nerven.StringParser.Core.Build;

var builder = new StringParserBuilder
    {
        PreSteps =
            {
                NullableParseStep.Default, // Adds Nullable<> suppoort
            },
        Steps =
            {
                EnumParseStep.Ordinal, // Parses enums
                CustomParseStep.Define(typeof(ushort), _s => // Custom parsing
                    {
                        ushort _value;
                        return ushort.TryParse(_s, out _value)
                            ? ParseStep.Valid((ushort)(_value + 1))
                            : ParseStep.InvalidString();
                    }),
            },
        PostSteps =
            {
                ConvertibleParseStep.Default, // Parses most .NET primitives and other 
            },
    };

var stringParser = builder.Build();

stringParser.Parse<bool?>("True"); // -> true
stringParser.Parse<UriKind>("RelativeOrAbsolute")); // -> UriKind.RelativeOrAbsolute
stringParser.Parse<ushort?>("0"); // -> 1
````
