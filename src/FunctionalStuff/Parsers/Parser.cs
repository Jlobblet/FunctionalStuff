using System;
using System.Collections.Generic;
using System.Linq;
using FunctionalStuff.Option;
using FunctionalStuff.Result;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace FunctionalStuff.Parsers
{
    public record Parser<T>(
        Func<ParserState, Result<ParserOk<T>, ParserError>> ParserFunc,
        string Label)
    {
        public Result<ParserOk<T>, ParserError> Run(ParserState input) => ParserFunc(input);

        public Parser<T> WithLabel(string label) => new(ParserFunc, label);

        public static Parser<char> Satisfy(Func<char, bool> predicate, string label)
        {
            Result<ParserOk<char>, ParserError> Inner(ParserState input)
            {
                var (remaining, character) = input.NextChar();
                return character switch
                       {
                           None<char> =>
                               Result<ParserOk<char>, ParserError>.Error(new ParserError("", "",
                                                                             ParserPosition.FromState(input))),
                           Some<char> c =>
                               c.Value switch
                               {
                                   { } v when predicate(v) =>
                                       Result<ParserOk<char>, ParserError>.Ok(new ParserOk<char>(v, remaining)),
                                   var _ => Result<ParserOk<char>, ParserError>.Error(new ParserError(
                                       label, $"Unexpected {c}",
                                       ParserPosition.FromState(input))),
                               },
                           _ => throw new ArgumentOutOfRangeException(),
                       };
            }

            return new Parser<char>(Inner, label);
        }

        public static Parser<char> PChar(char charToMatch) => Satisfy(c => c == charToMatch, charToMatch.ToString());

        public static Parser<char> IsDigit() => Satisfy(char.IsDigit, "digit");

        public static Parser<char> IsWhitespace() => Satisfy(char.IsWhiteSpace, "whitespace");

        public static Parser<char> IsNonWhitespace() => Satisfy(c => !char.IsWhiteSpace(c), "non-whitespace");

        public Parser<TOut> Bind<TOut>(Func<T, Parser<TOut>> binder)
        {
            Result<ParserOk<TOut>, ParserError> Inner(ParserState input) =>
                Run(input) switch
                {
                    Error<ParserOk<T>, ParserError> e => Result<ParserOk<TOut>, ParserError>.Error(e.Value),
                    Ok<ParserOk<T>, ParserError> o    => binder(o.Value.Value).Run(o.Value.State),
                    var _                             => throw new ArgumentOutOfRangeException(),
                };

            return new Parser<TOut>(Inner, Label);
        }

        public static Parser<T> Return(T x)
        {
            Result<ParserOk<T>, ParserError> Inner(ParserState input) =>
                Result<ParserOk<T>, ParserError>.Ok(new ParserOk<T>(x, input));

            return new Parser<T>(Inner, "Return");
        }

        public Parser<TOut> Map<TOut>(Func<T, TOut> mapping) => Bind(t => Parser<TOut>.Return(mapping(t)));

        public static Parser<TOut> Apply2<T1, T2, TOut>(
            Func<T1, T2, TOut> funcParser,
            Parser<T1> xParser,
            Parser<T2> yParser) =>
            Parser<Func<T1, T2, TOut>>.Return(funcParser)
                                      .Bind(f => xParser.Bind(x => yParser.Bind(y => Parser<TOut>.Return(f(x, y)))));

        public Parser<(T, T2)> AndThen<T2>(Parser<T2> other) =>
            Bind(r1 =>
                     other.Bind(r2 =>
                                    Parser<(T, T2)>.Return((r1, r2))))
                .WithLabel($"{Label} and then {other.Label}");

        public Parser<T> AndThenLeft<T2>(Parser<T2> other) =>
            Bind(r1 =>
                     other.Bind(_ =>
                                    Return(r1)));

        public Parser<T2> AndThenRight<T2>(Parser<T2> other) =>
            Bind(_ =>
                     other.Bind(Parser<T2>.Return));

        public Parser<T> Between<TLeft, TRight>(Parser<TLeft> left, Parser<TRight> right) =>
            left.AndThenRight(this).AndThenLeft(right);

        public Parser<T> OrElse(Parser<T> other)
        {
            Result<ParserOk<T>, ParserError> Inner(ParserState input) =>
                Run(input) switch
                {
                    Ok<ParserOk<T>, ParserError> o  => o,
                    Error<ParserOk<T>, ParserError> => other.Run(input),
                    var _                           => throw new ArgumentOutOfRangeException(),
                };

            return new Parser<T>(Inner, $"{Label} or else {other.Label}");
        }

        public static Parser<T> Choice(IEnumerable<Parser<T>> parsers)
        {
            var enumerable = parsers as Parser<T>[] ?? parsers.ToArray();
            return enumerable.Aggregate((acc, elt) => acc.OrElse(elt))
                             .WithLabel($"choice of {string.Join(", ", enumerable.Select(p => p.ToString()))}");
        }

        public static Parser<char> AnyOf(IEnumerable<char> chars)
        {
            var enumerable = chars as char[] ?? chars.ToArray();
            return Parser<char>.Choice(enumerable.Select(PChar)).WithLabel($"any of {string.Join(", ", enumerable)}");
        }

        public static Parser<IEnumerable<T>> Sequence(IEnumerable<Parser<T>> parsers) =>
            parsers.Aggregate(Parser<IEnumerable<T>>.Return(new List<T>()),
                              (current, parser) => Apply2((front, last) => front.Append(last), current, parser));

        public static Parser<string> PStringClassic(string str) =>
            Parser<char>.Sequence(str.Select(PChar))
                        .Map(i => new string(i.ToArray()))
                        .WithLabel($"string {str}");

        public static Parser<string> PString(string str)
        {
            Result<ParserOk<string>, ParserError> Inner(ParserState state) =>
                state.CurrentLine()
                     .Filter(l => l.Substring(state.Position.Column).StartsWith(str)) switch
                {
                    Some<string> => Result<ParserOk<string>, ParserError>.Ok(
                        new ParserOk<string>(
                            str,
                            state with {Position = state.Position with {Column = state.Position.Column + str.Length}})),
                    None<string> => Result<ParserOk<string>, ParserError>.Error(
                        new ParserError(str, "", ParserPosition.FromState(state))),
                    var _ => throw new ArgumentOutOfRangeException(),
                };

            return new Parser<string>(Inner, $"string {str}");
        }

        private (IEnumerable<T>, ParserState) ParseZeroPlus(ParserState state)
        {
            var parsed = new List<T>();
            while (true)
            {
                var result = Run(state);
                if (result is Ok<ParserOk<T>, ParserError> o)
                {
                    parsed.Add(o.Value.Value);
                    state = o.Value.State;
                }
                else { break; }
            }

            return (parsed, state);
        }

        public Parser<IEnumerable<T>> Many() =>
            new(
                i => Result<ParserOk<IEnumerable<T>>, ParserError>.Ok(
                    ParserOk<IEnumerable<T>>.FromTuple(ParseZeroPlus(i))),
                "");

        public Parser<IEnumerable<T>> Many1() => AndThen(Many()).Map(x => (IEnumerable<T>) x.Item2.Prepend(x.Item1));

        public Parser<IEnumerable<T>> SepBy1<T2>(Parser<T2> sep) =>
            AndThen(sep.AndThenRight(this).Many()).Map(t => (IEnumerable<T>) t.Item2.Prepend(t.Item1));

        public Parser<IEnumerable<T>> SepBy<T2>(Parser<T2> sep) =>
            SepBy1(sep).OrElse(Parser<IEnumerable<T>>.Return(Array.Empty<T>()));

        public Parser<Option<T>> Opt()
        {
            var some = Map(Option<T>.Some);
            var none = Parser<Option<T>>.Return(Option<T>.None());
            return some.OrElse(none);
        }

        public static Parser<int> PInt()
        {
            static int ResultToInt((Option<char> sign, IEnumerable<char> digits) tup)
            {
                var signMultiplier = tup.sign is Some<char> ? -1 : 1;
                return signMultiplier * int.Parse(new string(tup.digits.ToArray()));
            }

            var digits = IsDigit().Many1();
            var signed = PChar('-').Opt().AndThen(digits);
            return signed.Map(ResultToInt).WithLabel("int");
        }

        public static Parser<int> PIntRange(int lower, int upper)
        {
            var label = $"integer range {lower}-{upper}";

            Result<ParserOk<int>, ParserError> Inner(ParserState state) =>
                PInt()
                    .Run(state)
                    .Bind(i => i.Value switch
                               {
                                   { } outside when outside < lower || outside > upper =>
                                       Result<ParserOk<int>, ParserError>.Error(
                                           new ParserError(label, $"{outside} is outside range",
                                                           ParserPosition.FromState(state))),
                                   { } _ => Result<ParserOk<int>, ParserError>.Ok(i),
                               });

            return new Parser<int>(Inner, label);
        }

        public Parser<IEnumerable<T>> Until<T2>(Parser<T2> until)
        {
            var label = $"until {until.Label}";

            Result<ParserOk<IEnumerable<T>>, ParserError> Inner(ParserState state)
            {
                var results = new List<T>();
                while (true)
                {
                    if (until.Run(state) is Ok<ParserOk<T2>, ParserError> o)
                    {
                        state = o.Value.State;
                        break;
                    }

                    if (Run(state) is Ok<ParserOk<T>, ParserError> r)
                    {
                        state = r.Value.State;
                        results.Add(r.Value.Value);
                    }
                    else { break; }
                }

                return Result<ParserOk<IEnumerable<T>>, ParserError>.Ok(new ParserOk<IEnumerable<T>>(results, state));
            }

            return new Parser<IEnumerable<T>>(Inner, label);
        }

        public Parser<IEnumerable<T>> Until1<T2>(Parser<T2> until) =>
            AndThen(Until(until)).Map(t => (IEnumerable<T>) t.Item2.Prepend(t.Item1));

        public Parser<IEnumerable<T>> NTimes(int n) => Sequence(Enumerable.Repeat(this, n));
    }

    public static class ParserExtensions
    {
        public static Parser<TOut> Apply<T, TOut>(this Parser<Func<T, TOut>> funcParser, Parser<T> valueParser) =>
            funcParser.Bind(f => valueParser.Bind(v => Parser<TOut>.Return(f(v))));
    }
}
