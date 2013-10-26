using System.Collections.Generic;

namespace SparseArray
{

    class ZNode<T> : INodeInternal<T>
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


        public INodeInternal<T> Set(uint xlen, uint ylen, uint zlen, uint x, uint y, uint z, T item)
        {
            var nl = left;
            var nr = right;

            uint leftLen = LeftLength(xlen, ylen, zlen);

            if (z < leftLen)
            {
                nl = left.Set(xlen, ylen, leftLen, x, y, z, item);
            }
            else
            {
                uint rightLen = RightLength(xlen, ylen, zlen);

                nr = right.Set(xlen, ylen, rightLen, x, y, z - leftLen, item);
            }

            if (nl.IsLeaf && nr.IsLeaf)
            {
                var comparer = EqualityComparer<T>.Default;

                var leftItem = nl.Get(1, 1, 1, 0, 0, 0);
                var rightItem = nr.Get(1, 1, 1, 0, 0, 0);

                if (comparer.Equals(leftItem, rightItem))
                {
                    return nl;
                }
            }
            else if (object.ReferenceEquals(nl, left) && object.ReferenceEquals(nr, right))
            {
                return this;
            }

            return new ZNode<T>(nl, nr);
        }

        public bool IsLeaf
        {
            get
            {
                return false;
            }
        }

        public Dimension Slice
        {
            get
            {
                return Dimension.Z;
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
