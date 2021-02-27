namespace FunctionalStuff.Option
{
    public class Some<T> : Option<T>
    {
        public readonly T Value;
        
        public Some(T value)
        {
            Value = value;
        }
    }
}
