namespace FunctionalStuff.Result
{
    public class Ok<T, TErr> : Result<T, TErr>
    {
        public readonly T Value;

        public Ok(T value) => Value = value;
    }
}
