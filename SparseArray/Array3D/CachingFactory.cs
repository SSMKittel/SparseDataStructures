using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

namespace Sparse.Array3D
{
    [Obsolete("TODO the internal caching behaviour leaks memory like a sieve, fix it")]
    public class CachingFactory<T> : INodeFactory<T>
    {
        private CLNCachingFactory<T> cachingFactory;

        public CachingFactory(uint maxHeight)
        {
            this.cachingFactory = new CLNCachingFactory<T>(maxHeight);
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
            IComputedLengthNode<T> node = this.cachingFactory.Get(item);
            return new NodeView<T>(node, xlen, ylen, zlen);
        }

        public INode<T> Get(Dimension dim, INode<T> left, INode<T> right)
        {
            var nl = this.Unwrap(left);
            var nr = this.Unwrap(right);

            var node = this.cachingFactory.Get(dim, nl, nr);

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

            var newTree = this.cachingFactory.WithSet(innerNode, xlen, ylen, zlen, x, y, z, item);
            return new NodeView<T>(newTree, xlen, ylen, zlen);
        }
    }
}
