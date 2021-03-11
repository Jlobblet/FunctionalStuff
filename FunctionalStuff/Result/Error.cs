namespace FunctionalStuff.Result
{
    public class Error<T, TErr> : Result<T, TErr>
    {
        public readonly TErr Value;

        public Error(TErr value) => Value = value;
    }
}
