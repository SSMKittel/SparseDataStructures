using System.Collections.Generic;
using System;

namespace Sparse.Array3D
{
    public class SimpleFactory<T> : INodeInternalFactory<T>
    {
        public SimpleFactory() { }

        public INodeInternal<T> Get(T item)
        {
            return new Leaf<T>(item);
        }

        public INodeInternal<T> Get(NodeType type, INodeInternal<T> left, INodeInternal<T> right)
        {
            if (type == NodeType.Leaf)
            {
                throw new ArgumentException("Must not be a leaf node");
            }

            if ((left.Type == NodeType.Leaf) && (right.Type == NodeType.Leaf))
            {
                // Both children are leaf nodes
                var comparer = EqualityComparer<T>.Default;

                var leftItem = left.Get(1, 1, 1, 0, 0, 0);
                var rightItem = right.Get(1, 1, 1, 0, 0, 0);

                // Check if both leaf-children store the same value
                if (comparer.Equals(leftItem, rightItem))
                {
                    // They store the same value, this node and its children should be merged into one leaf node.
                    // Since the leaf nodes only store the value, we can just return either one of the children and have the same effect.
                    return left;
                }
            }

            if (type == NodeType.X)
            {
                return new XNode<T>(left, right);
            }
            else if (type == NodeType.Y)
            {
                return new YNode<T>(left, right);
            }
            else
            {
                return new ZNode<T>(left, right);
            }
        }

    }
}
