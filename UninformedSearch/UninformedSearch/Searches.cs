using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UninformedSearch
{
    class Solution
    {
        public TileBoard StartBoard;
        public TileBoard EndBoard;
        public int ExpandedNodes = 0;
        public long TimeElapsed;
        public List<BoardNode> MoveList;
        public bool IsSolved = false;
        private Stopwatch watch;

        public void StartTime()
        {
            TimeElapsed = 0;
            watch = Stopwatch.StartNew();
        }

        public void StopTime()
        {
            watch.Stop();
            TimeElapsed = watch.ElapsedMilliseconds;
        }

        public void Print()
        {
            if (!IsSolved)
            {
                Console.WriteLine(" Could not solve!");
                return;
            }

            Console.WriteLine("\n Starting Configuration:\n");
            StartBoard.Print();
            Console.WriteLine("");
            Console.WriteLine(" "+TimeElapsed+" milliseconds to find a solution");
            Console.WriteLine(" "+ExpandedNodes+" nodes expanded");
            Console.WriteLine(" Solution Path:\n ");
            Console.Write("Move the given number to '_' in the following order:\n ");
            var first = true;
            foreach (var node in MoveList)
            {
                if (node.TileMoved == '#')
                {
                    Console.Write(" Only showing last 100 moves: ");
                    continue;
                }
                
                if (node.TileMoved != '!')
                {
                    if (first)
                        first = false;
                    else
                        Console.Write(",");
                    Console.Write(" " + node.TileMoved);
                }
            }

            //debug
            if (false)
            {
                foreach (var node in MoveList)
                {
                    Console.WriteLine("");
                    node.Board.Print();
                }
            }

            Console.WriteLine("\n");
            Console.WriteLine(" Solved Puzzle:\n");
            EndBoard.Print();
            Console.WriteLine("");
        }
    }

    class Searches
    {
        private INodeList openNodes;
        private Solution solution;
        private Dictionary<string, BoardNode> visitedNodes;
        private bool bidirectional = false;
        private INodeList openNodesReverse;


        private void Start(TileBoard inputBoard)
        {
            //initialize new solution
            solution = new Solution();
            solution.StartBoard = inputBoard.Copy();
            solution.StartTime();

            //initialize new visited nodes table
            visitedNodes = new Dictionary<string, BoardNode>();
            
            //add input board to open node list
            var startNode = new BoardNode(null,inputBoard,'!');
            visitedNodes.Add(inputBoard.ToString(), startNode);
            solution.ExpandedNodes++;
            openNodes.Push(startNode);
            //start solving
            while (true)
            {
                if (Solve())
                    break;
            }
        }

        private bool Solve()
        {
            //if there are no more open nodes, the puzzle is not solveable
            if (openNodes.Count() == 0)
                return true;

            //get the next node to be evaluated
            var currentNode = openNodes.Pop();

            //check if current node is the solved node
            if (currentNode.Board.IsSolved())
            {
                solution.StopTime();
                solution.IsSolved = true;
                solution.EndBoard = currentNode.Board;

                //get complete move path
                solution.MoveList = new List<BoardNode> {currentNode};
                while (currentNode.Previous != null)
                {
                    solution.MoveList.Insert(0,currentNode.Previous);
                    if (solution.MoveList.Count >= 100)
                    {
                        currentNode.Previous.TileMoved = '#';
                        break;
                    }
                    currentNode = currentNode.Previous;
                }

                return true;
            }

            //get adjacent nodes
            var adjacents = currentNode.Board.GetAdjacents('_');

            //if adjacent node hasn't been visited, add it to open nodes
            foreach (var tile in adjacents)
            {
                var newBoard = currentNode.Board.Copy();
                newBoard.Switch('_',tile);
                var newBoardKey = newBoard.ToString();
                if (visitedNodes.ContainsKey(newBoardKey))
                {
                    //if new cost to get to this board is less than existing cost, replace it's previous node
                    var newCost = currentNode.Cost + 1;
                    if (visitedNodes[newBoardKey].Cost > newCost)
                    {
                        visitedNodes[newBoardKey].Previous = currentNode;
                        visitedNodes[newBoardKey].TileMoved = tile;
                        visitedNodes[newBoardKey].Cost = newCost;
                    }
                    continue;
                }
                solution.ExpandedNodes++;
                var newNode = new BoardNode(currentNode, newBoard, tile);
                visitedNodes.Add(newBoardKey, newNode);
                openNodes.Push(newNode);
            }
            return false;
        }

        public void BreadthFirstSearch(TileBoard inputBoard)
        {
            openNodes = new NodeQueue();
            Start(inputBoard);
            Console.WriteLine("Breadth First Search: ");
            solution.Print();
            Console.WriteLine("----------------------------");
        }

        public void DepthFirstSearch(TileBoard inputBoard)
        {
            openNodes = new NodeStack();
            Start(inputBoard);
            Console.WriteLine("Depth First Search: ");
            solution.Print();
            Console.WriteLine("----------------------------");
        }

        public void DepthLimitedSearch(TileBoard inputBoard)
        {
            openNodes = new NodeDepthStack(31, false);
            Start(inputBoard);
            Console.WriteLine("Depth Limited Search: (with a depth limit of 31)");
            solution.Print();
            Console.WriteLine("----------------------------");
        }

        public void IterativeDeepeningSearch(TileBoard inputBoard)
        {
            openNodes = new NodeDepthStack(1, true);
            Start(inputBoard);
            Console.WriteLine("Iterative Deepening Search: ");
            solution.Print();
            Console.WriteLine("----------------------------");
        }

        public void BidirectionalSearch(TileBoard inputBoard)
        {
            bidirectional = true;

            openNodes = new NodeQueue();
            openNodesReverse = new NodeQueue();

            Start(inputBoard);
            Console.WriteLine("Bidirectional Search: ");
            solution.Print();
            Console.WriteLine("----------------------------");

            bidirectional = false;
        }
    }


    
}
