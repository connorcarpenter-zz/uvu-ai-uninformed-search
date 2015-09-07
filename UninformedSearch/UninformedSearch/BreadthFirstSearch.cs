using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using static UninformedSearch.BreadthFirstSearch;

namespace UninformedSearch
{
    class BoardNode
    {
        public readonly BoardNode Previous;
        public readonly TileBoard Board;
        public char TileMoved;
        public int Cost;

        public BoardNode(BoardNode previous, TileBoard board, char TileMoved)
        {
            this.Previous = previous;
            this.Board = board;
            this.TileMoved = TileMoved;
            if (previous == null)
                Cost = 0;
            else Cost = previous.Cost + 1;
        }
    }

    interface INodeList
    {
        void Push(BoardNode node);
        BoardNode Pop();
    }

    class NodeQueue : INodeList
    {
        private Queue<BoardNode> queue; 

        public void Push(BoardNode node)
        {
            queue.Enqueue(node);
        }

        public BoardNode Pop()
        {
            return queue.Dequeue();
        }
    }

    class NodeStack : INodeList
    {
        private Stack<BoardNode> stack;

        public void Push(BoardNode node)
        {
            stack.Push(node);
        }

        public BoardNode Pop()
        {
            return stack.Pop();
        }
    }

    class NodeDepthStack : INodeList
    {
        private Stack<BoardNode> stack;
        private Queue<BoardNode> queue;
        public int Depth;

        public NodeDepthStack(int depth)
        {
            Depth = depth;
        }

        public void Push(BoardNode node)
        {
            if(node.Cost <= Depth)
                stack.Push(node);
            else
                queue.Enqueue(node);
        }

        public BoardNode Pop()
        {
            if(stack.Count>0)
                return stack.Pop();
            else
            {
                SetDepth(Depth+1);
                return stack.Pop();
            }
        }

        public void SetDepth(int newDepth)
        {
            Depth = newDepth;
            var newQueue = new Queue<BoardNode>();

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if(node.Cost <= Depth)
                    stack.Push(node);
                else
                    newQueue.Enqueue(node);
            }

            queue = newQueue;
        }
    }

    class DepthFirstSearch : BreadthFirstSearch
    {
        public override void Solve(TileBoard inputBoard)
        {
            Solve(inputBoard, new NodeStack());
        }
    }

    class BreadthFirstSearch
    {
        public virtual void Solve(TileBoard inputBoard)
        {
            Solve(inputBoard, new NodeQueue());
        }

        public static void Solve(TileBoard inputBoard, INodeList openNodes)
        {
            
        }
    }

    
}
