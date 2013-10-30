using System.Collections.Generic;

namespace Sparse.Array3D
{
    public class Leaf<T> : INodeInternal<T>
    {
        private readonly T item;

        public Leaf(T item)
        {
            this.item = item;
        }

        public T Get(uint xlen, uint ylen, uint zlen, uint x, uint y, uint z)
        {
            // Doesn't matter what the dimensions of this node is, only the value is relevant
            return item;
        }

        // Create a new node split along the longest axis with leaf children of this leaf node
        public INodeInternal<T> Split(uint xlen, uint ylen, uint zlen)
        {
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


        public INodeInternal<T> Set(INodeInternalFactory<T> factory, uint xlen, uint ylen, uint zlen, uint x, uint y, uint z, T item)
        {
            var comparer = EqualityComparer<T>.Default;

            if (comparer.Equals(this.item, item))
            {
                // Trying to set the item to the same thing
                return this;
            }
            else if (xlen == 1 && ylen == 1 && zlen == 1)
            {
                // We have exactly one position, set the new value
                return factory.Get(item);
            }
            else
            {
                // We need to set a new value somewhere in this node.  Split this leaf node and keep trying to find a spot
                var node = Split(xlen, ylen, zlen);
                return node.Set(factory, xlen, ylen, zlen, x, y, z, item);
            }
        }

        public NodeType Type
        {
            get
            {
                return NodeType.Leaf;
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
