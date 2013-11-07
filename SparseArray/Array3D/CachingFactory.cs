using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

namespace Sparse.Array3D
{
    class NodeComparer<T> : IEqualityComparer<IComputedLengthNode<T>>
    {
        public bool Equals(IComputedLengthNode<T> x, IComputedLengthNode<T> y)
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

        public int GetHashCode(IComputedLengthNode<T> node)
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
    public class CachingFactory<T> : INodeFactory<T>
    {
        private Dictionary<IComputedLengthNode<T>, int> heights;
        private Dictionary<IComputedLengthNode<T>, IComputedLengthNode<T>> nodes;
        private Dictionary<T, IComputedLengthNode<T>> leaves;

        private readonly uint maxHeight;

        public CachingFactory(uint maxHeight)
        {
            heights = new Dictionary<IComputedLengthNode<T>, int>();
            nodes = new Dictionary<IComputedLengthNode<T>, IComputedLengthNode<T>>(new NodeComparer<T>());
            leaves = new Dictionary<T, IComputedLengthNode<T>>();

            this.maxHeight = maxHeight;
        }

        // Create a new node split along the longest axis with leaf children of this leaf node
        private IComputedLengthNode<T> Split(IComputedLengthNode<T> node, uint xlen, uint ylen, uint zlen)
        {
            if (xlen >= ylen && xlen >= zlen)
            {
                return new XNode<T>(node, node);
            }
            else if (ylen >= xlen && ylen >= zlen)
            {
                return new YNode<T>(node, node);
            }
            else
            {
                return new ZNode<T>(node, node);
            }
        }

        // Return a new tree with {item} set at the specified position
        private IComputedLengthNode<T> WithSet(IComputedLengthNode<T> node, uint xlen, uint ylen, uint zlen, uint x, uint y, uint z, T item)
        {
            NodeType type = node.Type;

            if (type == NodeType.Leaf)
            {
                var comparer = EqualityComparer<T>.Default;
                var nodeItem = node.Get(1, 1, 1, 0, 0, 0);

                if (comparer.Equals(nodeItem, item))
                {
                    // Trying to set the item to the same thing
                    return node;
                }
                else if (xlen == 1 && ylen == 1 && zlen == 1)
                {
                    // We have exactly one position, set the new value
                    return this.Get(item);
                }
                else
                {
                    // We need to set a new value somewhere in this node.  Split this leaf node and keep trying to find a spot
                    var splitNode = this.Split(node, xlen, ylen, zlen);
                    return this.WithSet(splitNode, xlen, ylen, zlen, x, y, z, item);
                }
            }
            else
            {
                var left = node.Left;
                var right = node.Right;

                var nl = left;
                var nr = right;

                uint leftLen = node.LeftLength(xlen, ylen, zlen);
                uint rightLen = node.RightLength(xlen, ylen, zlen);

                Dimension dimension;

                if (type == NodeType.X)
                {
                    dimension = Dimension.X;

                    if (x < leftLen)
                    {
                        nl = this.WithSet(left, leftLen, ylen, zlen, x, y, z, item);
                    }
                    else
                    {
                        nr = this.WithSet(right, rightLen, ylen, zlen, x - leftLen, y, z, item);
                    }
                }
                else if (type == NodeType.Y)
                {
                    dimension = Dimension.Y;

                    if (y < leftLen)
                    {
                        nl = this.WithSet(left, xlen, leftLen, zlen, x, y, z, item);
                    }
                    else
                    {
                        nr = this.WithSet(right, xlen, rightLen, zlen, x, y - leftLen, z, item);
                    }
                }
                else
                {
                    dimension = Dimension.Z;

                    if (z < leftLen)
                    {
                        nl = this.WithSet(left, xlen, ylen, leftLen, x, y, z, item);
                    }
                    else
                    {
                        nr = this.WithSet(right, xlen, ylen, rightLen, x, y, z - leftLen, item);
                    }
                }

                if (object.ReferenceEquals(nl, left) && object.ReferenceEquals(nr, right))
                {
                    // There was no change
                    return node;
                }

                // One of the children changed, transfer the changes up the tree
                return this.Get(dimension, nl, nr);
            }
        }

        private int GetHeight(IComputedLengthNode<T> node)
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

        private bool IsCached(IComputedLengthNode<T> node)
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

        private IComputedLengthNode<T> Get(T item)
        {
            IComputedLengthNode<T> leaf;
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

        private IComputedLengthNode<T> Get(Dimension dim, IComputedLengthNode<T> left, IComputedLengthNode<T> right)
        {
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

            IComputedLengthNode<T> test = Create(dim, left, right);

            IComputedLengthNode<T> node;
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

        private IComputedLengthNode<T> Create(Dimension dim, IComputedLengthNode<T> left, IComputedLengthNode<T> right)
        {
            if (dim == Dimension.X)
            {
                return new XNode<T>(left, right);
            }
            else if (dim == Dimension.Y)
            {
                return new YNode<T>(left, right);
            }
            else
            {
                return new ZNode<T>(left, right);
            }
        }

        private void Cache(IComputedLengthNode<T> node)
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

        private IComputedLengthNode<T> Unwrap(INode<T> node)
        {
            if (node == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                NodeView<T> nv = (NodeView<T>)node;
                // TODO Check it was created by this factory
                return nv.Internal;
            }
        }

        public INode<T> Get(T item, uint xlen, uint ylen, uint zlen)
        {
            IComputedLengthNode<T> node = this.Get(item);
            return new NodeView<T>(node, xlen, ylen, zlen);
        }

        public INode<T> Get(Dimension dim, INode<T> left, INode<T> right)
        {
            var nl = this.Unwrap(left);
            var nr = this.Unwrap(right);

            var node = this.Get(dim, nl, nr);

            if (dim == Dimension.X)
            {
                return new NodeView<T>(node, left.XLength + right.XLength, left.YLength, left.ZLength);
            }
            else if (dim == Dimension.Y)
            {
                return new NodeView<T>(node, left.XLength, left.YLength + right.YLength, left.ZLength);
            }
            else
            {
                return new NodeView<T>(node, left.XLength, left.YLength, left.ZLength + right.ZLength);
            }
        }

        public INode<T> WithSet(INode<T> node, uint x, uint y, uint z, T item)
        {
            var innerNode = this.Unwrap(node);

            uint xlen = node.XLength;
            uint ylen = node.YLength;
            uint zlen = node.ZLength;

            var newTree = this.WithSet(innerNode, xlen, ylen, zlen, x, y, z, item);
            return new NodeView<T>(newTree, xlen, ylen, zlen);
        }
    }
}
