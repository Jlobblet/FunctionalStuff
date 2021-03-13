using System;
using System.Diagnostics;
using System.Linq;
using FunctionalStuff.Parsers;

namespace Parsers
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var pany = Parser<char>.Satisfy(c => true, "any");
            var phash = Parser<char>.PChar('#');
            var username = pany.Until(phash).Map(cs => new string(cs.ToArray()));
            var discriminator = Parser<char>.IsDigit().NTimes(4).Map(cs => new string(cs.ToArray()));

            var discordUsername = username.AndThen(discriminator).Map(DiscordUser.FromTuple);

            var pquote = Parser<char>.PChar('"');
            var pnonquote = Parser<char>.PChar('\\').AndThenRight(Parser<char>.PChar('"'))
                                        .OrElse(Parser<char>.Satisfy(c => c != '"', "non-\""));
            var pnonwhitespace = Parser<char>.IsNonWhitespace();
            var pargs = pquote.AndThenRight(pnonquote.Until1(pquote))
                             .OrElse(pnonwhitespace.Many())
                             .Map(chars => new string(chars.ToArray()))
                             .SepBy1(Parser<char>.IsWhitespace().Many1());
            
            const string testString = @"arg1 ""arg \""number\"" 2"" arg3";

            var stopwatch = Stopwatch.StartNew();

            // var result = discordUsername.Run(ParserState.FromString("Jlobblet#0621"));
            
            var result = pargs.Run(ParserState.FromString(testString));

            stopwatch.Stop();
            Console.WriteLine($"{result.Map(r => string.Join(", ", r.Value))}\n in {stopwatch.ElapsedMilliseconds}ms");
        }

        private record DiscordUser(string Username, string Discriminator)
        {
            public static DiscordUser FromTuple((string username, string discriminator) tuple) =>
                new(tuple.username, tuple.discriminator);
        }

        private record CommandInput(string Command, string[] Args)
        {
            public static CommandInput FromTuple((string username, string[] args) tuple) =>
                new(tuple.username, tuple.args);
        }
    }
}
