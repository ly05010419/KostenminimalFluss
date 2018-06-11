using namespaceAlgorithmus;
using System;

namespace KostenminimalFluss
{
    class Program
    {

        static void Main(string[] args)
        {
            Algorithmus algorithmus = new Algorithmus();
             
            //algorithmus.zeitOfAlgorithmus(@"../../KostenminimalFluss/Kostenminimal1.txt", "kostenminimalFluss", true);
            //algorithmus.zeitOfAlgorithmus(@"../../KostenminimalFluss/Kostenminimal2.txt", "kostenminimalFluss", true);
            //algorithmus.zeitOfAlgorithmus(@"../../KostenminimalFluss/Kostenminimal3.txt", "kostenminimalFluss",true);
           // algorithmus.zeitOfAlgorithmus(@"../../KostenminimalFluss/Kostenminimal4.txt", "kostenminimalFluss", true);

            algorithmus.zeitOfAlgorithmus(@"../../KostenminimalFluss/KostenminimalTest1.txt", "kostenminimalFluss", true);
            algorithmus.zeitOfAlgorithmus(@"../../KostenminimalFluss/KostenminimalTest2.txt", "kostenminimalFluss", true);
            algorithmus.zeitOfAlgorithmus(@"../../KostenminimalFluss/KostenminimalTest3.txt", "kostenminimalFluss", true);
           

            Console.WriteLine("\n");
            Console.ReadLine();
        }

    }
}
