using System;

namespace FunctionalStuff.Option
{
    public class Option<T>
    {
        public Option<TOut> Map<TOut>(Func<T, TOut> func)
        {
            return this switch
                   {
                       None<T> => new None<TOut>(),
                       Some<T> s => new Some<TOut>(func(s.Value)),
                       _ => throw new ArgumentOutOfRangeException()
                   };
        }

        public Option<TOut> Bind<TOut>(Func<T, Option<TOut>> func)
        {
            return this switch
                   {
                       None<T> => new None<TOut>(),
                       Some<T> s => func(s.Value),
                       _ => throw new ArgumentOutOfRangeException()
                   };
        }

        public TOut Fold<TOut>(TOut state, Func<TOut, T, TOut> func)
        {
            return this switch
                   {
                       None<T> => state,
                       Some<T> s => func(state, s.Value),
                       _ => throw new ArgumentOutOfRangeException()
                   };
        }

        public T DefaultValue(T def)
        {
            return this switch
                   {
                       None<T> => def,
                       Some<T> s => s.Value,
                       _ => throw new ArgumentOutOfRangeException()
                   };
        }

        public static Option<T> FromNullable(T input)
        {
            return input switch
                   {
                       null => new None<T>(),
                       { } some => new Some<T>(some),
                   };
        }

        public static bool IsNone(Option<T> option)
        {
            return option is None<T>;
        }

    }
}
