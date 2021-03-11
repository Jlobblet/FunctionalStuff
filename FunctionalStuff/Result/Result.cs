using System;

namespace FunctionalStuff.Result
{
    public abstract class Result<T, TErr>
    {
        public static Result<T, TErr> Ok(T value) => Ok<T, TErr>.Create(value);

        public static Result<T, TErr> Error(TErr value) => Error<T, TErr>.Create(value);

        public Result<TOut, TErr> Map<TOut>(Func<T, TOut> mapping) =>
            this switch
            {
                Error<T, TErr> e => Result<TOut, TErr>.Error(
                    e.Value),
                Ok<T, TErr> o => Result<TOut, TErr>.Ok(
                    mapping(o.Value)),
                var _ => throw
                    new ArgumentOutOfRangeException(),
            };

        public Result<TOut, TErr> Bind<TOut>(Func<T, Result<TOut, TErr>> binder) =>
            this switch
            {
                Error<T, TErr> e => Result<TOut, TErr>.Error(e.Value),
                Ok<T, TErr> o    => binder(o.Value),
                var _            => throw new ArgumentOutOfRangeException(),
            };
    }
}
