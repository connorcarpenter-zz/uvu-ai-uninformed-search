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
            else
            {
                tileBoard = new TileBoard();
                tileBoard.Randomize('_');
            }

            BreadthFirstSearch.Solve(tileBoard.Copy());

            Console.ReadLine();
        }
    }

}
