using System.Collections.Generic;
using System.Linq;

namespace UninformedSearch
{
    class BoardNode
    {
        public BoardNode Previous;
        public readonly TileBoard Board;
        public char TileMoved;
        public int Cost;
        public bool Reverse = false;

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
        int Count();
        bool Upgrade(BoardNode node);
    }

    class NodeQueue : INodeList
    {
        private Queue<BoardNode> queue = new Queue<BoardNode>();

        public void Push(BoardNode node)
        {
            queue.Enqueue(node);
        }

        public BoardNode Pop()
        {
            return queue.Dequeue();
        }

        public int Count()
        {
            return queue.Count();
        }

        public bool Upgrade(BoardNode node)
        {
            return false;
        }
    }

    class NodeStack : INodeList
    {
        private Stack<BoardNode> stack = new Stack<BoardNode>();

        public void Push(BoardNode node)
        {
            stack.Push(node);
        }

        public BoardNode Pop()
        {
            return stack.Pop();
        }

        public int Count()
        {
            return stack.Count();
        }

        public bool Upgrade(BoardNode node)
        {
            return false;
        }
    }

    class NodeDepthStack : INodeList
    {
        private Stack<BoardNode> stack;
        private List<BoardNode> queue;
        public int Depth;
        public bool CanDeepen = false;

        public NodeDepthStack(int depth, bool canDeepen = false)
        {
            Depth = depth;
            CanDeepen = canDeepen;
            stack = new Stack<BoardNode>();
            queue = new List<BoardNode>();
        }

        public void Push(BoardNode node)
        {
            if (node.Cost <= Depth)
                stack.Push(node);
            else
                queue.Insert(0,node);
        }

        public BoardNode Pop()
        {
            var result = stack.Pop();
            if (CanDeepen && stack.Count == 0)
                SetDepth(Depth + 1);
            return result;
        }

        public void SetDepth(int newDepth)
        {
            Depth = newDepth;
            var newQueue = new List<BoardNode>();

            while (queue.Count > 0)
            {
                var index = queue.Count-1;
                var node = queue[index];
                queue.RemoveAt(index);
                if (node.Cost <= Depth)
                    stack.Push(node);
                else
                    newQueue.Insert(0,node);
            }

            queue = newQueue;
        }

        public int Count()
        {
            return stack.Count();
        }

        public bool Upgrade(BoardNode node)
        {
            if (CanDeepen || node.Cost > Depth)
                return false;

            if (queue.Contains(node))
                queue.Remove(node);
            Push(node);
            return node.Cost + 1 <= Depth;
        }
    }
}
