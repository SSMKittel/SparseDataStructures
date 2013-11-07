using System.Collections.Generic;

namespace Sparse.Array3D
{
    internal class ZNode<T> : IComputedLengthNode<T>
    {
        private readonly IComputedLengthNode<T> left;
        private readonly IComputedLengthNode<T> right;

        public ZNode(IComputedLengthNode<T> left, IComputedLengthNode<T> right)
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

        public IComputedLengthNode<T> Left
        {
            get
            {
                return left;
            }
        }

        public IComputedLengthNode<T> Right
        {
            get
            {
                return right;
            }
        }
    }
}
