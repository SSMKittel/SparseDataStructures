using System.Collections.Generic;

namespace Sparse.Array3D
{
    public class XNode<T> : INodeInternal<T>
    {
        private readonly INodeInternal<T> left;
        private readonly INodeInternal<T> right;

        public XNode(INodeInternal<T> left, INodeInternal<T> right)
        {
            this.left = left;
            this.right = right;
        }

        public T Get(uint xlen, uint ylen, uint zlen, uint x, uint y, uint z)
        {
            uint leftLen = LeftLength(xlen, ylen, zlen);

            // Choose which child to traverse down
            if (x < leftLen)
            {
                return left.Get(leftLen, ylen, zlen, x, y, z);
            }
            else
            {
                uint rightLen = RightLength(xlen, ylen, zlen);

                return right.Get(rightLen, ylen, zlen, x - leftLen, y, z);
            }
        }


        public INodeInternal<T> Set(INodeInternalFactory<T> factory, uint xlen, uint ylen, uint zlen, uint x, uint y, uint z, T item)
        {
            var nl = left;
            var nr = right;

            uint leftLen = LeftLength(xlen, ylen, zlen);

            // Choose which child to traverse down
            if (x < leftLen)
            {
                nl = left.Set(factory, leftLen, ylen, zlen, x, y, z, item);
            }
            else
            {
                uint rightLen = RightLength(xlen, ylen, zlen);

                nr = right.Set(factory, rightLen, ylen, zlen, x - leftLen, y, z, item);
            }

            if (object.ReferenceEquals(nl, left) && object.ReferenceEquals(nr, right))
            {
                // There was no change
                return this;
            }

            // One of the children changed, transfer the changes up the tree
            return factory.Get(NodeType.X, nl, nr);
        }

        public NodeType Type
        {
            get
            {
                return NodeType.X;
            }
        }

        public uint LeftLength(uint xlen, uint ylen, uint zlen)
        {
            return (xlen + 1) / 2;
        }

        public uint RightLength(uint xlen, uint ylen, uint zlen)
        {
            return xlen - LeftLength(xlen, ylen, zlen);
        }

        public INodeInternal<T> Left
        {
            get
            {
                return left;
            }
        }

        public INodeInternal<T> Right
        {
            get
            {
                return right;
            }
        }
    }
}
