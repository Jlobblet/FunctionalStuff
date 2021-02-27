using System;
using FunctionalStuff.Option;

namespace FunctionalStuff
{
    public static class Program
    {
        private static Option<int> TryDivide(int num, int den)
        {
            if (den == 0)
            {
                return None<int>.Create();
            }

            return Some<int>.Create(num / den);
        }

        static void Main(string[] args)
        {
            var contentPackage =
                new ContentPackage("cp.xml");
            
            Console.WriteLine("hi");
        }
    }
}
