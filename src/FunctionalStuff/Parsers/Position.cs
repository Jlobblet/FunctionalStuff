namespace FunctionalStuff.Parsers
{
    public record Position(int Line, int Column)
    {
        public static Position InitialPosition() => new(0, 0);

        public Position IncrementColumn() => new(Line, Column + 1);

        public Position IncrementLine() => new(Line + 1, 0);
    }
}
