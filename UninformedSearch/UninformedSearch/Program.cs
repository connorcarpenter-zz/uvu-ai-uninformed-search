using System;
using System.IO;

namespace UninformedSearch
{
    class Program
    {
        private const bool DEBUG = true;

        static void Main()
        {
            TileBoard tileBoard;
            if (!DEBUG)
            {
                var sr = new StreamReader("../../input.txt");
                var input = sr.ReadToEnd();
                input = input.Replace("\r\n", "");

                tileBoard = new TileBoard(input);
            }

            while (Console.ReadLine() != "q")
            {
                if (DEBUG)
                {
                    tileBoard = new TileBoard();
                    tileBoard.Randomize('_');
                }
                var searches = new Searches();
                searches.BreadthFirstSearch(tileBoard.Copy());
                searches.DepthFirstSearch(tileBoard.Copy());
                searches.DepthLimitedSearch(tileBoard.Copy());
                searches.IterativeDeepeningSearch(tileBoard.Copy());
                searches.BidirectionalSearch(tileBoard.Copy());
            }
            Console.ReadLine();
        }
    }

}
