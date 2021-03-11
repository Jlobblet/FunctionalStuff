namespace FunctionalStuff.Option
{
    public sealed class Some<T> : Option<T>
    {
        public readonly T Value;

        private Some(T value) => Value = value;

        public static Option<T> Create(T value) => new Some<T>(value);
    }
}
