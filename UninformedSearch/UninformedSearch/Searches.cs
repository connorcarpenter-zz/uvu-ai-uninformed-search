using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
                    Console.Write(" Only showing last 100 moves: ...,");
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

            //for bidirectional search, add end node to reverse open list
            if (bidirectional)
            {
                var endBoard = new TileBoard("_12345678");
                var endNode = new BoardNode(null, endBoard, '!');
                endNode.Reverse = true;
                visitedNodes.Add(endBoard.ToString(), endNode);
                solution.ExpandedNodes++;
                openNodesReverse.Push(endNode);
            }

            //start solving
            while (true)
            {
                if (Solve())
                    break;
                if (bidirectional)
                {
                    if (Solve(true))
                        break;
                }
            }
        }

        private bool Solve(bool reverse = false)
        {
            var openList = openNodes;
            if (reverse)
                openList = openNodesReverse;

            //if there are no more open nodes, the puzzle is not solveable
            if (openList.Count() == 0)
                return true;

            //get the next node to be evaluated
            var currentNode = openList.Pop();

            //check if current node is the solved node
            if (!reverse)
                if(currentNode.Board.IsSolved())
                {
                    CreateSolution(currentNode);
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
                    var visitedNode = visitedNodes[newBoardKey];
                    //if new cost to get to this board is less than existing cost, replace it's previous node
                    if (visitedNode.Reverse == reverse)
                    {
                        var newCost = currentNode.Cost + 1;
                        if (visitedNode.Cost > newCost)
                        {
                            visitedNode.Previous = currentNode;
                            visitedNode.TileMoved = tile;
                            visitedNode.Cost = newCost;
                            if(openList.Upgrade(visitedNode))
                                UpgradeChildren(openList,visitedNode);
                        }
                    }
                    else
                    {
                        //piece this thang together, solved it!
                        if(reverse)
                            CreateBidirectionalSolution(visitedNode, currentNode, tile);
                        else
                            CreateBidirectionalSolution(currentNode, visitedNode, tile);
                        return true;
                    }
                    continue;
                }
                solution.ExpandedNodes++;
                var newNode = new BoardNode(currentNode, newBoard, tile);
                newNode.Reverse = reverse;
                visitedNodes.Add(newBoardKey, newNode);
                openList.Push(newNode);
            }
            return false;
        }

        private void UpgradeChildren(INodeList openList,BoardNode visitedNode)
        {
            return;

            foreach (var node in visitedNodes.Where(node => node.Value.Previous == visitedNode))
            {
                node.Value.Cost = visitedNode.Cost + 1;
                if (openList.Upgrade(node.Value))
                    UpgradeChildren(openList, node.Value);
            }
        }

        private void CreateBidirectionalSolution(BoardNode forwardNode,BoardNode reverseNode,char tile)
        {
            solution.StopTime();
            solution.IsSolved = true;

            //get move path to forwardNode
            var currentNode = forwardNode;
            solution.MoveList = new List<BoardNode> { currentNode };
            while (currentNode.Previous != null)
            {
                solution.MoveList.Insert(0, currentNode.Previous);
                currentNode = currentNode.Previous;
            }

            //get move path from forwardNode to reverseNode
            while (true)
            {
                var connectNode = new BoardNode(forwardNode, reverseNode.Board, tile);
                tile = reverseNode.TileMoved;
                solution.MoveList.Add(connectNode);
                if (reverseNode.Previous == null)
                    break;
                reverseNode = reverseNode.Previous;
            }

            solution.EndBoard = solution.MoveList.Last().Board;
        }

        private void CreateSolution(BoardNode currentNode)
        {
            solution.StopTime();
            solution.IsSolved = true;
            solution.EndBoard = currentNode.Board;

            //get complete move path
            solution.MoveList = new List<BoardNode> {currentNode};
            while (currentNode.Previous != null)
            {
                solution.MoveList.Insert(0, currentNode.Previous);
                if (solution.MoveList.Count >= 100)
                {
                    currentNode.Previous.TileMoved = '#';
                    break;
                }
                currentNode = currentNode.Previous;
            }
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
            openNodes = new NodeDepthStack(27, false);
            Start(inputBoard);
            Console.WriteLine("Depth Limited Search: (with a depth limit of 27)");
            solution.Print();
            Console.WriteLine("----------------------------");
        }

        public void IterativeDeepeningSearch(TileBoard inputBoard)
        {
            var dataStructure = new NodeDepthStack(1, true);
            openNodes = dataStructure;
            Start(inputBoard);
            Console.WriteLine("Iterative Deepening Search: (Final depth was " + dataStructure.Depth + " )");
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
