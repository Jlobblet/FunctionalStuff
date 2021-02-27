using System;
using System.Collections.Generic;

namespace FunctionalStuff.Option
{
    public class Option<T>
    {
        protected Option()
        {
        }

        public static Option<T> Some(T value)
        {
            return Some<T>.Create(value);
        }

        public static Option<T> None()
        {
            return None<T>.Create();
        }
        
        public static Option<T> FromNullable(T input)
        {
            return input switch
                   {
                       null => None(),
                       { } some => Some(some)
                   };
        }

        public Option<TOut> Bind<TOut>(Func<T, Option<TOut>> func)
        {
            return this switch
                   {
                       None<T> => Option<TOut>.None(),
                       Some<T> s => func(s.Value),
                       _ => throw new ArgumentOutOfRangeException()
                   };
        }

        public bool Contains(T value)
        {
            return this is Some<T> s && s.Value.Equals(value);
        }
        
        public int Count()
        {
            return this is Some<T> ? 1 : 0;
        }

        public T DefaultValue(T def)
        {
            return this switch
                   {
                       None<T> => def,
                       Some<T> s => s.Value,
                       _ => throw new ArgumentException()
                   };
        }

        public T DefaultWith(Func<T> defThunk)
        {
            return this switch
                   {
                       Some<T> s => s.Value,
                       None<T> => defThunk(),
                       _ => throw new ArgumentException(),
                   };
        }
        
        public bool Exists(Func<T, bool> predicate)
        {
            return this is Some<T> s && predicate(s.Value);
        }

        public Option<T> Filter(Func<T, bool> predicate)
        {
            return this switch
                   {
                       None<T> n => n,
                       Some<T> s => predicate(s.Value) ? s : None(),
                       _ => throw new ArgumentException()
                   };
        }

        public TOut Fold<TOut>(TOut state, Func<TOut, T, TOut> folder)
        {
            return this switch
                   {
                       None<T> => state,
                       Some<T> s => folder(state, s.Value),
                       _ => throw new ArgumentOutOfRangeException()
                   };
        }

        public TOut FoldBack<TOut>(Func<T, TOut, TOut> folder, TOut state)
        {
            return this switch
                   {
                       None<T> => state,
                       Some<T> s => folder(s.Value, state),
                       _ => throw new ArgumentException()
                   };
        }
        
        public bool IsNone()
        {
            return this is None<T>;
        }

        public bool IsSome()
        {
            return this is Some<T>;
        }

        public void Iter(Action<T> action)
        {
            if (this is Some<T> s)
                action(s.Value);
        }
        
        public Option<TOut> Map<TOut>(Func<T, TOut> func)
        {
            return this switch
                   {
                       None<T> => Option<TOut>.None(),
                       Some<T> s => Option<TOut>.Some(func(s.Value)),
                       _ => throw new ArgumentOutOfRangeException()
                   };
        }

        public Option<TOut> Map2<T2, TOut>(Func<T, T2, TOut> mapping, Option<T2> other)
        {
            return (this, other) switch
                   {
                       (Some<T> s1, Some<T2> s2) => Option<TOut>.Some(mapping(s1.Value, s2.Value)),
                       _ => Option<TOut>.None(),
                   };
        }

        public Option<TOut> Map3<T2, T3, TOut>(Func<T, T2, T3, TOut> mapping, Option<T2> other1, Option<T3> other2)
        {
            return (this, other1, other2) switch
                   {
                       (Some<T> s1, Some<T2> s2, Some<T3> s3) =>
                           Option<TOut>.Some(mapping(s1.Value, s2.Value, s3.Value)),
                       _ => Option<TOut>.None(),
                   };
        }

        public Option<T> OrElse(Option<T> ifNone)
        {
            return IsNone() ? ifNone : this;
        }

        public Option<T> OrElseWith(Func<Option<T>> ifNoneThunk)
        {
            return IsNone() ? ifNoneThunk() : this;
        }

        public T[] ToArray()
        {
            if (this is Some<T> s)
            {
                return new[] {s.Value};
            }

            return Array.Empty<T>();
        }

        public List<T> ToList()
        {
            if (this is Some<T> s)
            {
                return new List<T> {s.Value};
            }

            return new List<T>();
        }
        
        public T UnwrapOr(Exception exception)
        {
            return this switch
                   {
                       Some<T> s => s.Value,
                       _ => throw exception,
                   };
        }
    }
}
