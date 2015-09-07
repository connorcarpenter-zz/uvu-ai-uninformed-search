using System;
using System.Collections.Generic;

namespace UninformedSearch
{

    class Coordinate
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool AreAdjacent(Coordinate a, Coordinate b)
        {
            var result = Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
            return (result == 1);
        }
    }

    class TileBoard
    {
        private char[][] _board =
        {
            new char[3],
            new char[3],
            new char[3]
        };

        private readonly Random _random = new Random();

        private const string AllowedChars = "_12345678";
        private const string SolvedBoard = "_12345678";

        public TileBoard()
        {
            SetBoard(AllowedChars);
        }

        public TileBoard(string input)
        {
            SetBoard(input);
        }

        private void SetBoard(string input)
        {
            var x = 0;
            var y = 0;
            foreach (var character in input)
            {
                _board[y][x] = character;
                x++;
                if (x > 2)
                {
                    x = 0;
                    y++;
                }
            }
        }

        public override string ToString()
        {
            var str = "";

            for (var y = 1; y <= 3; y++)
                for (var x = 1; x <= 3; x++)
                    str += GetTile(x, y);

            return str;
        }

        public char GetTile(int x, int y)
        {
            return _board[y - 1][x - 1];
        }

        private void SetTile(int x, int y, char str)
        {
            _board[y - 1][x - 1] = str;
        }

        public void Print()
        {
            for (var y = 1; y <= 3; y++)
            {
                for (var x = 1; x <= 3; x++)
                {
                    Console.Write("" + GetTile(x, y) + " ");
                }
                Console.WriteLine();
            }
        }

        public bool Equals(TileBoard otherBoard)
        {
            return ToString().Equals(otherBoard.ToString());
        }

        public TileBoard Copy()
        {
            return new TileBoard(ToString());
        }

        public bool IsSolved()
        {
            return ToString().Equals(SolvedBoard);
        }

        public Coordinate GetPosition(char tile)
        {
            if (!AllowedChars.Contains("" + tile))
                throw new Exception("Error in TileBoard.GetPosition(): Searching for string that cannot exist on the board");

            for (var x = 1; x <= 3; x++)
                for (var y = 1; y <= 3; y++)
                    if (GetTile(x, y).Equals(tile))
                        return new Coordinate(x, y);
            return null;
        }

        public List<char> GetAdjacents(int x, int y)
        {
            var result = new List<char>();

            if (x > 1) result.Add(GetTile(x - 1, y));
            if (x < 3) result.Add(GetTile(x + 1, y));
            if (y > 1) result.Add(GetTile(x, y - 1));
            if (y < 3) result.Add(GetTile(x, y + 1));

            return result;
        }

        public List<char> GetAdjacents(char tile)
        {
            var coord = GetPosition(tile);
            return GetAdjacents(coord.X, coord.Y);
        }

        public void Switch(char charA, char charB)
        {
            var coordA = GetPosition(charA);
            var coordB = GetPosition(charB);

            if (!Coordinate.AreAdjacent(coordA, coordB))
                throw new Exception("Error in TileBoard.Switch(): Strings to switch are not adjacent to one another");

            SetTile(coordA.X, coordA.Y, charB);
            SetTile(coordB.X, coordB.Y, charA);
        }

        public void Randomize(char tile)
        {
            for (var i = 0; i < 200; i++)
                MoveRandom(tile);
        }

        public void MoveRandom(char tile)
        {
            var adjacents = GetAdjacents('_');
            var index = _random.Next(0, adjacents.Count);
            Switch('_', adjacents[index]);
        }
    }
}
