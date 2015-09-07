using System;
using System.IO;
using System.Linq.Expressions;

namespace UninformedSearch
{
    class Program
    {
        private const bool DEBUG = false;

        static void Main(string[] args)
        {
            TileBoard tileBoard;
            
            if(!DEBUG)
            {
                string filePath;
                if (args.Length == 0)
                    filePath = "input.txt";
                else
                    filePath = args[0];
                string input = "_12345678";

                try
                {
                    var sr = new StreamReader(filePath);
                    input = sr.ReadToEnd();
                    input = input.Replace("\r\n", "");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                    return;
                }

                tileBoard = new TileBoard(input);
            }

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
    }

}
