using System;
using FunctionalStuff.Option;

namespace FunctionalStuff
{
    public static class Program
    {
        private static Option<int> TryDivide(int num, int den)
        {
            return den == 0 ? Option<int>.None() : Option<int>.Some(num / den);
        }

        static void Main(string[] args)
        {
            var contentPackage =
                new ContentPackage("cp.xml");
            
            Console.WriteLine("hi");
        }
    }
}
