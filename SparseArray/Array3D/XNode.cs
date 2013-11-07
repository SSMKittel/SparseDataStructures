using System.Collections.Generic;

namespace Sparse.Array3D
{
    internal class XNode<T> : IComputedLengthNode<T>
    {
        private readonly IComputedLengthNode<T> left;
        private readonly IComputedLengthNode<T> right;

        public XNode(IComputedLengthNode<T> left, IComputedLengthNode<T> right)
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
