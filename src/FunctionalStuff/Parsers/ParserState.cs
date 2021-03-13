using System;
using FunctionalStuff.Option;

namespace FunctionalStuff.Parsers
{
    public record ParserState(string[] Lines, Position Position)
    {
        public static ParserState FromString(string str) => new(str.Split("\n"), Position.InitialPosition());

        public Option<string> CurrentLine() =>
            Position.Line < Lines.Length
                ? Option<string>.Some(Lines[Position.Line])
                : Option<string>.None();

        private ParserState IncrementColumn() => new(Lines, Position.IncrementColumn());

        private ParserState IncrementLine() => new(Lines, Position.IncrementLine());

        public (ParserState, Option<char>) NextChar()
        {
            if (Position.Line >= Lines.Length) return (this, Option<char>.None());

            var currentLine = CurrentLine().UnwrapOr(new Exception());

            if (Position.Column < currentLine.Length)
                return (IncrementColumn(),
                        Option<char>.Some(currentLine[Position.Column]));

            return (IncrementLine(), Option<char>.None());
        }
    }
}
