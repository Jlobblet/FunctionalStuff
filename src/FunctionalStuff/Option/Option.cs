using System;
using System.Collections.Generic;

namespace FunctionalStuff.Option
{
    public abstract class Option<T> : IFunctor<T>
    {
        IFunctor<TOut> IFunctor<T>.Map<TOut>(Func<T, TOut> mapping) => Map(mapping);

        public static Option<T> Some(T value) => Some<T>.Create(value);

        public static Option<T> None() => None<T>.Create();

        public static Option<T> FromNullable(T input) =>
            input switch
            {
                null     => None(),
                { } some => Some(some),
            };

        public override string ToString() =>
            this switch
            {
                None<T>   => "None",
                Some<T> s => $"Some {s.Value.ToString()}",
                var _     => throw new ArgumentOutOfRangeException(),
            };

        public Option<TOut> Bind<TOut>(Func<T, Option<TOut>> func) =>
            this switch
            {
                None<T>   => Option<TOut>.None(),
                Some<T> s => func(s.Value),
                var _     => throw new ArgumentOutOfRangeException(),
            };

        public bool Contains(T value) => this is Some<T> s && s.Value.Equals(value);

        public int Count() => this is Some<T> ? 1 : 0;

        public T DefaultValue(T def) =>
            this switch
            {
                None<T>   => def,
                Some<T> s => s.Value,
                var _     => throw new ArgumentOutOfRangeException(),
            };

        public T DefaultWith(Func<T> defThunk) =>
            this switch
            {
                Some<T> s => s.Value,
                None<T>   => defThunk(),
                var _     => throw new ArgumentOutOfRangeException(),
            };

        public bool Exists(Func<T, bool> predicate) => this is Some<T> s && predicate(s.Value);

        public Option<T> Filter(Func<T, bool> predicate) =>
            this switch
            {
                None<T> n => n,
                Some<T> s => predicate(s.Value) ? s : None(),
                var _     => throw new ArgumentOutOfRangeException(),
            };

        public TOut Fold<TOut>(TOut state, Func<TOut, T, TOut> folder) =>
            this switch
            {
                None<T>   => state,
                Some<T> s => folder(state, s.Value),
                var _     => throw new ArgumentOutOfRangeException(),
            };

        public TOut FoldBack<TOut>(Func<T, TOut, TOut> folder, TOut state) =>
            this switch
            {
                None<T>   => state,
                Some<T> s => folder(s.Value, state),
                var _     => throw new ArgumentException(),
            };

        public bool IsNone() => this is None<T>;

        public bool IsSome() => this is Some<T>;

        public void Iter(Action<T> action)
        {
            if (this is Some<T> s)
                action(s.Value);
        }

        public Option<TOut> Map<TOut>(Func<T, TOut> mapping) =>
            this switch
            {
                None<T>   => Option<TOut>.None(),
                Some<T> s => Option<TOut>.Some(mapping(s.Value)),
                var _     => throw new ArgumentOutOfRangeException(),
            };

        public Option<TOut> Map2<T2, TOut>(Func<T, T2, TOut> mapping, Option<T2> other) =>
            (this, other) switch
            {
                (Some<T> s1, Some<T2> s2) => Option<TOut>.Some(mapping(s1.Value, s2.Value)),
                var _                     => Option<TOut>.None(),
            };

        public Option<TOut> Map3<T2, T3, TOut>(Func<T, T2, T3, TOut> mapping, Option<T2> other1, Option<T3> other2) =>
            (this, other1, other2) switch
            {
                (Some<T> s1, Some<T2> s2, Some<T3> s3) =>
                    Option<TOut>.Some(mapping(s1.Value, s2.Value, s3.Value)),
                var _ => Option<TOut>.None(),
            };

        public Option<T> OrElse(Option<T> ifNone) => IsNone() ? ifNone : this;

        public Option<T> OrElseWith(Func<Option<T>> ifNoneThunk) => IsNone() ? ifNoneThunk() : this;

        public T[] ToArray() => this is Some<T> s ? new[] {s.Value} : Array.Empty<T>();

        public List<T> ToList() => this is Some<T> s ? new List<T> {s.Value} : new List<T>();

        public T UnwrapOr(Exception exception) =>
            this switch
            {
                Some<T> s => s.Value,
                var _     => throw exception,
            };
    }
}
