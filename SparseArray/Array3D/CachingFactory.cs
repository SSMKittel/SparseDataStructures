using System.Collections.Generic;
using System;

namespace Sparse.Array3D
{
    public class CachingFactory<T> : INodeInternalFactory<T>
    {
        private Dictionary<INodeInternal<T>, int> heights;
        private Dictionary<Tuple<NodeType, INodeInternal<T>, INodeInternal<T>>, INodeInternal<T>> nodes;
        private Dictionary<T, INodeInternal<T>> leaves;

        public CachingFactory()
        {
            heights = new Dictionary<INodeInternal<T>, int>();
            nodes = new Dictionary<Tuple<NodeType, INodeInternal<T>, INodeInternal<T>>, INodeInternal<T>>();
            leaves = new Dictionary<T, INodeInternal<T>>();
        }

        private int GetHeight(INodeInternal<T> node)
        {
            if (node.Type == NodeType.Leaf)
            {
                return 1;
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

            var tup = new Tuple<NodeType, INodeInternal<T>, INodeInternal<T>>(type, left, right);

            INodeInternal<T> node;
            if (nodes.TryGetValue(tup, out node))
            {
                return node;
            }
            else
            {
                if (type == NodeType.X)
                {
                    node = new XNode<T>(left, right);
                }
                else if (type == NodeType.Y)
                {
                    node = new YNode<T>(left, right);
                }
                else
                {
                    node = new ZNode<T>(left, right);
                }
                Cache(node);
                return node;
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

                    if (newHeight <= 4)
                    {
                        var tup = new Tuple<NodeType, INodeInternal<T>, INodeInternal<T>>(node.Type, node.Left, node.Right);
                        nodes[tup] = node;
                        heights[node] = newHeight;
                    }
                }
            }
        }
    }
}
