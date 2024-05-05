using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns.Behavioral.IteratorPattern
{
    class Iterator
    {
        public static void Invoke()
        {
            var b1 = new BinaryNode<int>(2, new BinaryNode<int>(4), new BinaryNode<int>(5));
            var b2 = new BinaryNode<int>(3, new BinaryNode<int>(6), new BinaryNode<int>(7));
            var binaryTree = new BinaryNode<int>(1, b1, b2);

            foreach (var item in binaryTree)
            {
                Console.WriteLine(item);
            }

            var inorderIterator = new InorderIterator<int>(binaryTree);
            foreach (var item in inorderIterator.Inorder)
            {
                Console.WriteLine(item);
            }

            inorderIterator.Print();
        }
    }


    interface Iterator<T>
    {
        bool MoveNext(); // or void MoveNExt() & bool HasNext { get; set; }
        T Current { get; }
    }

    interface Iteratable<T>
    {
        Iterator<T> GetEnumerator();
    }

    interface INode<T>
    {
        T Value { get; }
    }

    interface IBinaryNode<T> : INode<T>
    {
        IBinaryNode<T> Parent { get; set; }
        IBinaryNode<T> Left { get; }
        IBinaryNode<T> Right { get; }
    }

    class BinaryNode<T> : IBinaryNode<T>, Iteratable<T>
    {
        public BinaryNode(T value)
        {
            Value = value;
        }

        public BinaryNode(T value, IBinaryNode<T> left, IBinaryNode<T> right) : this(value)
        {
            Left = left;
            Right = right;
            Parent = null;
            Left.Parent = Right.Parent = this;
        }

        public IBinaryNode<T> Left { get; private set; }
        public IBinaryNode<T> Right { get; private set; }
        public IBinaryNode<T> Parent { get; set; }

        public T Value { get; private set; }


        public Iterator<T> GetEnumerator()
        {
            return new InorderIterator<T>(this);
        }
    }

    class InorderIterator<T> : Iterator<T>
    {
        IBinaryNode<T> RootNode;

        bool isYieldedStart;
        public InorderIterator(IBinaryNode<T> rootNode)
        {
            RootNode = rootNode;
            CurrentNode = RootNode;
            while (CurrentNode.Left != null)
            {
                CurrentNode = CurrentNode.Left;
            }
        }

        private IBinaryNode<T> CurrentNode { get; set; }

        public T Current => CurrentNode.Value;

        public bool MoveNext()
        {

            if (!isYieldedStart)
            {
                isYieldedStart = true;
                return true;
            }

            if (CurrentNode.Right != null)
            {
                CurrentNode = CurrentNode.Right;
                while (CurrentNode.Left != null)
                    CurrentNode = CurrentNode.Left;

                return true;
            }
            else
            {
                var p = CurrentNode.Parent;
                while (p != null && CurrentNode == p.Right)
                {
                    CurrentNode = p;
                    p = p.Parent;
                }

                CurrentNode = p;

                return CurrentNode != null;
            }
        }

        public IEnumerable<T> Inorder
        {
            get
            {
                IEnumerable<T> Traverse(IBinaryNode<T> node)
                {
                    if (node == null)
                    {
                        yield return default(T);
                    }

                    if (node.Left != null)
                    {
                        foreach (var leftNodeValue in Traverse(node.Left))
                        {
                            yield return leftNodeValue;
                        }
                    }

                    yield return node.Value;

                    if (node.Right != null)
                    {
                        foreach (var rightNodeValue in Traverse(node.Right))
                        {
                            yield return rightNodeValue;
                        }
                    }

                }
                return Traverse(RootNode);
            }

        }

        private void Print(IBinaryNode<T> rootNode)
        {
            if (rootNode == null)
                return;

            Print(rootNode.Left);
            Console.WriteLine(rootNode.Value);
            Print(rootNode.Right);
        }

        public void Print()
        {
            Print(RootNode);
        }
    }

    public class Ex<T> : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator()
        {
            return new ExEnumerator<T>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class ExEnumerator<T> : IEnumerator<T>
    {
        public T Current => throw new NotImplementedException();

        object IEnumerator.Current => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }

}
