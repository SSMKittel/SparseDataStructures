using System.Collections.Generic;

namespace SparseArray
{
    class Leaf<T> : INodeInternal<T>
    {
        private readonly T item;

        public Leaf(T item)
        {
            this.item = item;
        }

        public bool IsLeaf
        {
            get
            {
                return true;
            }
        }

        public T Get(uint xlen, uint ylen, uint zlen, uint x, uint y, uint z)
        {
            return item;
        }

        public INodeInternal<T> Split(uint xlen, uint ylen, uint zlen)
        {
            // Create a new node split along the longest axis with leaf children of this item
            if (xlen >= ylen && xlen >= zlen)
            {
                return new XNode<T>(this, this);
            }
            else if (ylen >= xlen && ylen >= zlen)
            {
                return new YNode<T>(this, this);
            }
            else
            {
                return new ZNode<T>(this, this);
            }
        }


        public INodeInternal<T> Set(uint xlen, uint ylen, uint zlen, uint x, uint y, uint z, T item)
        {
            var comparer = EqualityComparer<T>.Default;

            if (comparer.Equals(this.item, item))
            {
                // Trying to set the item to the same thing
                return this;
            }
            else if (xlen == 1 && ylen == 1 && zlen == 1)
            {
                return new Leaf<T>(item);
            }
            else
            {
                var node = Split(xlen, ylen, zlen);
                return node.Set(xlen, ylen, zlen, x, y, z, item);
            }
        }

        public Dimension Slice
        {
            get
            {
                return Dimension.None;
            }
        }

        public uint LeftLength(uint xlen, uint ylen, uint zlen)
        {
            return 0;
        }

        public uint RightLength(uint xlen, uint ylen, uint zlen)
        {
            return 0;
        }

        public INodeInternal<T> Left
        {
            get
            {
                return null;
            }
        }

        public INodeInternal<T> Right
        {
            get
            {
                return null;
            }
        }
    }
}
