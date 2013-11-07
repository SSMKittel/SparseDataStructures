using System.Collections.Generic;

namespace Sparse.Array3D
{
    internal class YNode<T> : IComputedLengthNode<T>
    {
        private readonly IComputedLengthNode<T> left;
        private readonly IComputedLengthNode<T> right;

        public YNode(IComputedLengthNode<T> left, IComputedLengthNode<T> right)
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
