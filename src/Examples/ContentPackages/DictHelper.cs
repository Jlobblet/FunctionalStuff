using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using FunctionalStuff.Option;

namespace ContentPackages
{
    public static class DictHelper
    {
        private static ImmutableDictionary<TKey, TValue>.Builder Add<TKey, TValue>(
            this ImmutableDictionary<TKey, TValue>.Builder builder,
            TKey key,
            TValue value)
        {
            builder.Add(key, value);
            return builder;
        }

        public static Option<ImmutableDictionary<TKey, TValue>> TryToImmutableDictionary<TSource, TKey, TValue>(
            this IEnumerable<TSource> enumerable,
            Func<TSource, Option<TKey>> keySelector,
            Func<TSource, Option<TValue>> valueSelector)
        {
            Option<ImmutableDictionary<TKey, TValue>.Builder> Folder(
                Option<ImmutableDictionary<TKey, TValue>.Builder> builder,
                TSource element)
            {
                return builder.Bind(b =>
                                        keySelector(element).Map2((k, v) => Add(b, k, v), valueSelector(element)));
            }

            var builder =
                Option<ImmutableDictionary<TKey, TValue>.Builder>.Some(ImmutableDictionary
                                                                           .CreateBuilder<TKey, TValue>());
            return enumerable.Aggregate(builder, Folder).Map(b => b.ToImmutableDictionary());
        }

        public static Option<ImmutableDictionary<TKey, TValue>> TryToImmutableDictionary<TSource, TKey, TValue>(
            this IEnumerable<TSource> enumerable,
            Func<TSource, TKey> keySelector,
            Func<TSource, Option<TValue>> valueSelector)
        {
            Option<ImmutableDictionary<TKey, TValue>.Builder> Folder(
                Option<ImmutableDictionary<TKey, TValue>.Builder> builder,
                TSource element)
            {
                return builder.Bind(b =>
                                        valueSelector(element).Map(v => Add(b, keySelector(element), v)));
            }

            var builder =
                Option<ImmutableDictionary<TKey, TValue>.Builder>.Some(ImmutableDictionary
                                                                           .CreateBuilder<TKey, TValue>());
            return enumerable.Aggregate(builder, Folder).Map(b => b.ToImmutableDictionary());
        }

        public static Option<ImmutableDictionary<TKey, TValue>> TryToImmutableDictionary<TSource, TKey, TValue>(
            this IEnumerable<TSource> enumerable,
            Func<TSource, Option<TKey>> keySelector,
            Func<TSource, TValue> valueSelector)
        {
            Option<ImmutableDictionary<TKey, TValue>.Builder> Folder(
                Option<ImmutableDictionary<TKey, TValue>.Builder> builder,
                TSource element)
            {
                return builder.Bind(b =>
                                        keySelector(element).Map(k => Add(b, k, valueSelector(element))));
            }

            var builder =
                Option<ImmutableDictionary<TKey, TValue>.Builder>.Some(ImmutableDictionary
                                                                           .CreateBuilder<TKey, TValue>());
            return enumerable.Aggregate(builder, Folder).Map(b => b.ToImmutableDictionary());
        }
    }
}
