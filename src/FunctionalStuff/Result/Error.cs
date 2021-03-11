namespace FunctionalStuff.Result
{
    public class Error<T, TErr> : Result<T, TErr>
    {
        public readonly TErr Value;

        private Error(TErr value) => Value = value;

        public static Result<T, TErr> Create(TErr value) => new Error<T, TErr>(value);
    }
}
