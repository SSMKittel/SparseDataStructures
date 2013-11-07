using System.Collections.Generic;
using System;

namespace Sparse.Array3D
{
    public class SimpleFactory<T> : INodeFactory<T>
    {
        public SimpleFactory() { }

        public INodeInternal<T> Get(T item)
        {
            return new Leaf<T>(item);
        }

        public INodeInternal<T> Get(Dimension dim, INodeInternal<T> left, INodeInternal<T> right)
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


        // Create a new node split along the longest axis with leaf children of this leaf node
        public INodeInternal<T> Split(INodeInternal<T> node, uint xlen, uint ylen, uint zlen)
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
        public INodeInternal<T> WithSet(INodeInternal<T> node, uint xlen, uint ylen, uint zlen, uint x, uint y, uint z, T item)
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
    }
}
