using System;

namespace ContentPackages
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var contentPackage =
                new ContentPackage("cp.xml");

            foreach (var (key, value) in contentPackage.Files) Console.WriteLine($"{key} : {value}");
        }
    }
}
