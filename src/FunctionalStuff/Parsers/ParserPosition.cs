using FunctionalStuff.Option;

namespace FunctionalStuff.Parsers
{
    public record ParserPosition(Option<string> CurrentLine, Position Position)
    {
        public static ParserPosition FromState(ParserState state) => new(state.CurrentLine(), state.Position);
    }
}
