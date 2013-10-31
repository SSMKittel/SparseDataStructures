using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

namespace Sparse.Array3D
{
    class NodeComparer<T> : IEqualityComparer<INodeInternal<T>>
    {
        public bool Equals(INodeInternal<T> x, INodeInternal<T> y)
        {
            if (x.Type == y.Type)
            {
                if (x.Type == NodeType.Leaf)
                {
                    var comparer = EqualityComparer<T>.Default;

                    var xItem = x.Get(1, 1, 1, 0, 0, 0);
                    var yItem = y.Get(1, 1, 1, 0, 0, 0);

                    return comparer.Equals(xItem, yItem);
                }
                else
                {
                    return object.ReferenceEquals(x.Left, y.Left) && object.ReferenceEquals(x.Right, y.Right);
                }
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(INodeInternal<T> node)
        {

            NodeType nt = node.Type;
            if (nt == NodeType.Leaf)
            {
                var comparer = EqualityComparer<T>.Default;

                return comparer.GetHashCode(node.Get(1, 1, 1, 0, 0, 0));
            }
            else
            {
                unchecked
                {
                    int d = 31 * RuntimeHelpers.GetHashCode(node.Left) + 53 * RuntimeHelpers.GetHashCode(node.Right);

                    if (nt == NodeType.X)
                    {
                        return 67 * d;
                    }
                    else if (nt == NodeType.Y)
                    {
                        return 89 * d;
                    }
                    else
                    {
                        return 113 * d;
                    }
                }
            }
        }
    }

    [Obsolete("TODO the internal caching behaviour leaks memory like a sieve, fix it")]
    public class CachingFactory<T> : INodeInternalFactory<T>
    {
        private Dictionary<INodeInternal<T>, int> heights;
        private Dictionary<INodeInternal<T>, INodeInternal<T>> nodes;
        private Dictionary<T, INodeInternal<T>> leaves;

        private readonly uint maxHeight;

        public CachingFactory(uint maxHeight)
        {
            heights = new Dictionary<INodeInternal<T>, int>();
            nodes = new Dictionary<INodeInternal<T>, INodeInternal<T>>(new NodeComparer<T>());
            leaves = new Dictionary<T, INodeInternal<T>>();

            this.maxHeight = maxHeight;
        }

        private int GetHeight(INodeInternal<T> node)
        {
            if (node.Type == NodeType.Leaf)
            {
                return 0;
            }
            else
            {
                return heights[node];
            }
        }

        private bool IsCached(INodeInternal<T> node)
        {
            if (node.Type == NodeType.Leaf)
            {
                return leaves.ContainsKey(node.Get(1, 1, 1, 0, 0, 0));
            }
            else
            {
                return heights.ContainsKey(node);
            }
        }

        public INodeInternal<T> Get(T item)
        {
            INodeInternal<T> leaf;
            if (leaves.TryGetValue(item, out leaf))
            {
                return leaf;
            }
            else
            {
                leaf = new Leaf<T>(item);
                Cache(leaf);
                return leaf;
            }
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

            INodeInternal<T> test = Create(type, left, right);

            INodeInternal<T> node;
            if (nodes.TryGetValue(test, out node))
            {
                return node;
            }
            else
            {
                Cache(test);
                return test;
            }
        }

        private INodeInternal<T> Create(NodeType type, INodeInternal<T> left, INodeInternal<T> right)
        {
            if (type == NodeType.X)
            {
                return new XNode<T>(left, right);
            }
            else if (type == NodeType.Y)
            {
                return new YNode<T>(left, right);
            }
            else if (type == NodeType.Z)
            {
                return new ZNode<T>(left, right);
            }
            else
            {
                throw new ArgumentException("Must not be a leaf node");
            }
        }

        private void Cache(INodeInternal<T> node)
        {
            if (node.Type == NodeType.Leaf)
            {
                T item = node.Get(1, 1, 1, 0, 0, 0);
                leaves[item] = node;
            }
            else
            {
                bool bothCached = IsCached(node.Left) && IsCached(node.Right);

                if (bothCached)
                {
                    int newHeight = Math.Max(GetHeight(node.Left), GetHeight(node.Right)) + 1;

                    if (newHeight <= maxHeight)
                    {
                        nodes[node] = node;
                        heights[node] = newHeight;
                    }
                }
            }
        }
    }
}
