using System;

namespace FunctionalStuff.Parsers
{
    public record ParserOk<T>(T Value, ParserState State)
    {
        public override string ToString() => Value.ToString();

        public static ParserOk<T> FromTuple((T value, ParserState state) tuple) => new(tuple.value, tuple.state);
    }

    public record ParserError(string Label, string Error, ParserPosition Position)
    {
        public override string ToString()
        {
            var positionLine = Position.Position.Line;
            var positionColumn = Position.Position.Column;
            var caret = "^".PadLeft(positionColumn + 1);
            var currentLine = Position.CurrentLine.UnwrapOr(new ArgumentNullException());
            return
                $"Line {positionLine} column {positionColumn}: error parsing {Label}\n{currentLine}\n{caret}\n";
        }
    }
}
