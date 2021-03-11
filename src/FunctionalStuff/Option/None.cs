namespace FunctionalStuff.Option
{
    public sealed class None<T> : Option<T>
    {
        private None()
        {
        }

        public static Option<T> Create() => new None<T>();
    }
}
