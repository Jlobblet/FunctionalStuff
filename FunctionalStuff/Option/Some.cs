namespace FunctionalStuff.Option
{
    public class Some<T> : Option<T>
    {
        public readonly T Value;
        
        protected Some(T value)
        {
            Value = value;
        }

        public static Option<T> Create(T value)
        {
            return new Some<T>(value);
        }
    }
}
