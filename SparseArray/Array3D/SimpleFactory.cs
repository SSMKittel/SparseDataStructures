using System.Collections.Generic;
using System;

namespace Sparse.Array3D
{
    public class SimpleFactory<T> : INodeFactory<T>
    {
        private CLNSimpleFactory<T> simpleFactory;

        public SimpleFactory() 
        {
            this.simpleFactory = new CLNSimpleFactory<T>();
        }

        private IComputedLengthNode<T> Unwrap(INode<T> node)
        {
            if (node == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                NodeView<T> nv = (NodeView<T>) node;
                // TODO Check it was created by this factory
                return nv.Internal;
            }
        }

        public INode<T> Get(T item, uint xlen, uint ylen, uint zlen)
        {
            IComputedLengthNode<T> node = this.simpleFactory.Get(item);
            return new NodeView<T>(node, xlen, ylen, zlen);
        }

        public INode<T> Get(Dimension dim, INode<T> left, INode<T> right)
        {
            var nl = this.Unwrap(left);
            var nr = this.Unwrap(right);

            var node = this.simpleFactory.Get(dim, nl, nr);

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

            var newTree = this.simpleFactory.WithSet(innerNode, xlen, ylen, zlen, x, y, z, item);
            return new NodeView<T>(newTree, xlen, ylen, zlen);
        }
    }
}
