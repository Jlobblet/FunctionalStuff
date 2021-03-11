using System;

namespace FunctionalStuff.Result
{
    public abstract class Result<T, TErr>
    {
        public static Result<T, TErr> Ok(T value) => new Ok<T, TErr>(value);

        public static Result<T, TErr> Error(TErr value) => new Error<T, TErr>(value);

        public Result<TOut, TErr> Map<TOut>(Func<T, TOut> mapping) => this switch
                                                                      {
                                                                          Error<T, TErr> e => new Error<TOut, TErr>(
                                                                              e.Value),
                                                                          Ok<T, TErr> o => new Ok<TOut, TErr>(
                                                                              mapping(o.Value)),
                                                                          _ => throw new ArgumentOutOfRangeException()
                                                                      };

        public Result<TOut, TErr> Bind<TOut>(Func<T, Result<TOut, TErr>> binder) => this switch
            {
                Error<T, TErr> e => new Error<TOut, TErr>(e.Value),
                Ok<T, TErr> o    => binder(o.Value),
                _                => throw new ArgumentOutOfRangeException()
            };
    }
}
