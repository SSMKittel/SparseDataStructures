using System.Collections.Generic;

namespace Sparse.Array3D
{

    public class ZNode<T> : INodeInternal<T>
    {
        private readonly INodeInternal<T> left;
        private readonly INodeInternal<T> right;

        public ZNode(INodeInternal<T> left, INodeInternal<T> right)
        {
            this.left = left;
            this.right = right;
        }

        public T Get(uint xlen, uint ylen, uint zlen, uint x, uint y, uint z)
        {
            uint leftLen = LeftLength(xlen, ylen, zlen);

            // Choose which child to traverse down
            if (z < leftLen)
            {
                return left.Get(xlen, ylen, leftLen, x, y, z);
            }
            else
            {
                uint rightLen = RightLength(xlen, ylen, zlen);

                return right.Get(xlen, ylen, rightLen, x, y, z - leftLen);
            }
        }


        public INodeInternal<T> Set(INodeInternalFactory<T> factory, uint xlen, uint ylen, uint zlen, uint x, uint y, uint z, T item)
        {
            var nl = left;
            var nr = right;

            uint leftLen = LeftLength(xlen, ylen, zlen);

            // Choose which child to traverse down
            if (z < leftLen)
            {
                nl = left.Set(factory, xlen, ylen, leftLen, x, y, z, item);
            }
            else
            {
                uint rightLen = RightLength(xlen, ylen, zlen);

                nr = right.Set(factory, xlen, ylen, rightLen, x, y, z - leftLen, item);
            }


            if (object.ReferenceEquals(nl, left) && object.ReferenceEquals(nr, right))
            {
                // There was no change
                return this;
            }

            // One of the children changed, transfer the changes up the tree
            return factory.Get(NodeType.Z, nl, nr);
        }

        public NodeType Type
        {
            get
            {
                return NodeType.Z;
            }
        }

        public uint LeftLength(uint xlen, uint ylen, uint zlen)
        {
            return (zlen + 1) / 2;
        }

        public uint RightLength(uint xlen, uint ylen, uint zlen)
        {
            return zlen - LeftLength(xlen, ylen, zlen);
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
