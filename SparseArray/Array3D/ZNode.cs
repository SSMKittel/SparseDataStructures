using System.Collections.Generic;

namespace Sparse.Array3D
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


        public INodeInternal<T> Set(uint xlen, uint ylen, uint zlen, uint x, uint y, uint z, T item)
        {
            var nl = left;
            var nr = right;

            uint leftLen = LeftLength(xlen, ylen, zlen);

            // Choose which child to traverse down
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
                // Both children are leaf nodes
                var comparer = EqualityComparer<T>.Default;

                var leftItem = nl.Get(1, 1, 1, 0, 0, 0);
                var rightItem = nr.Get(1, 1, 1, 0, 0, 0);

                // Check if both leaf-children store the same value
                if (comparer.Equals(leftItem, rightItem))
                {
                    // They store the same value, this node and its children should be merged into one leaf node.
                    // Since the leaf nodes only store the value, we can just return either one of the children and have the same effect.
                    return nl;
                }
            }
            else if (object.ReferenceEquals(nl, left) && object.ReferenceEquals(nr, right))
            {
                // There was no change
                return this;
            }

            // One of the children changed, transfer the changes up the tree
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
