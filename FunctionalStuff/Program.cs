using System;
using FunctionalStuff.Option;

namespace FunctionalStuff
{
    public static class Program
    {
        private static Option<int> TryDivide(int num, int den) =>
            den == 0 ? Option<int>.None() : Option<int>.Some(num / den);

        private static void Main(string[] args)
        {
            var contentPackage =
                new ContentPackage("cp.xml");

            foreach (var (key, value) in contentPackage.Files) Console.WriteLine($"{key} : {value}");
        }
    }
}
