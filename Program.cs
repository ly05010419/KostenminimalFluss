using namespaceAlgorithmus;
using System;

namespace KostenminimalFluss
{
    class Program
    {

        static void Main(string[] args)
        {
            Algorithmus algorithmus = new Algorithmus();

            algorithmus.zeitOfAlgorithmus(@"../../KostenminimalFluss/Kostenminimal1.txt", "cycleCancelling", true);
            algorithmus.zeitOfAlgorithmus(@"../../KostenminimalFluss/Kostenminimal2.txt", "cycleCancelling", true);
            algorithmus.zeitOfAlgorithmus(@"../../KostenminimalFluss/Kostenminimal3.txt", "cycleCancelling",true);
            algorithmus.zeitOfAlgorithmus(@"../../KostenminimalFluss/Kostenminimal4.txt", "cycleCancelling", true);

            //algorithmus.zeitOfAlgorithmus(@"../../KostenminimalFluss/KostenminimalTest1.txt", "cycleCancelling", true);
            //algorithmus.zeitOfAlgorithmus(@"../../KostenminimalFluss/KostenminimalTest2.txt", "cycleCancelling", true);
            //algorithmus.zeitOfAlgorithmus(@"../../KostenminimalFluss/KostenminimalTest3.txt", "cycleCancelling", true);
            //algorithmus.zeitOfAlgorithmus(@"../../KostenminimalFluss/KostenminimalTest4.txt", "cycleCancelling", true);



             algorithmus.zeitOfAlgorithmus(@"../../KostenminimalFluss/Kostenminimal1.txt", "successiveShortestPath", true);
             algorithmus.zeitOfAlgorithmus(@"../../KostenminimalFluss/Kostenminimal2.txt", "successiveShortestPath", true);
             algorithmus.zeitOfAlgorithmus(@"../../KostenminimalFluss/Kostenminimal3.txt", "successiveShortestPath", true);
             algorithmus.zeitOfAlgorithmus(@"../../KostenminimalFluss/Kostenminimal4.txt", "successiveShortestPath", true);

            //algorithmus.zeitOfAlgorithmus(@"../../KostenminimalFluss/KostenminimalTest1.txt", "successiveShortestPath", true);
            //algorithmus.zeitOfAlgorithmus(@"../../KostenminimalFluss/KostenminimalTest2.txt", "successiveShortestPath", true);
            //algorithmus.zeitOfAlgorithmus(@"../../KostenminimalFluss/KostenminimalTest3.txt", "successiveShortestPath", true);
            //algorithmus.zeitOfAlgorithmus(@"../../KostenminimalFluss/KostenminimalTest4.txt", "successiveShortestPath", true);

            Console.WriteLine("\n");
            Console.ReadLine();
        }

    }
}
