using System.Collections.Generic;

namespace Sparse.Array3D
{
    public class YNode<T> : INodeInternal<T>
    {
        private readonly INodeInternal<T> left;
        private readonly INodeInternal<T> right;

        public YNode(INodeInternal<T> left, INodeInternal<T> right)
        {
            this.left = left;
            this.right = right;
        }

        public T Get(uint xlen, uint ylen, uint zlen, uint x, uint y, uint z)
        {
            uint leftLen = LeftLength(xlen, ylen, zlen);

            // Choose which child to traverse down
            if (y < leftLen)
            {
                return left.Get(xlen, leftLen, zlen, x, y, z);
            }
            else
            {
                uint rightLen = RightLength(xlen, ylen, zlen);

                return right.Get(xlen, rightLen, zlen, x, y - leftLen, z);
            }
        }


        public INodeInternal<T> Set(INodeInternalFactory<T> factory, uint xlen, uint ylen, uint zlen, uint x, uint y, uint z, T item)
        {
            var nl = left;
            var nr = right;

            uint leftLen = LeftLength(xlen, ylen, zlen);

            // Choose which child to traverse down
            if (y < leftLen)
            {
                nl = left.Set(factory, xlen, leftLen, zlen, x, y, z, item);
            }
            else
            {
                uint rightLen = RightLength(xlen, ylen, zlen);

                nr = right.Set(factory, xlen, rightLen, zlen, x, y - leftLen, z, item);
            }

            if (object.ReferenceEquals(nl, left) && object.ReferenceEquals(nr, right))
            {
                // There was no change
                return this;
            }

            // One of the children changed, transfer the changes up the tree
            return factory.Get(NodeType.Y, nl, nr);
        }

        public NodeType Type
        {
            get
            {
                return NodeType.Y;
            }
        }

        public uint LeftLength(uint xlen, uint ylen, uint zlen)
        {
            return (ylen + 1) / 2;
        }

        public uint RightLength(uint xlen, uint ylen, uint zlen)
        {
            return ylen - LeftLength(xlen, ylen, zlen);
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
