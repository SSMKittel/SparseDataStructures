using System;

namespace SparseArray
{
    public class SparseArray3D<T>
    {
        private readonly uint xLength;
        private readonly uint yLength;
        private readonly uint zLength;

        private INodeInternal<T> root;

        public SparseArray3D(uint xlen, uint ylen, uint zlen)
        {
            this.xLength = xlen;
            this.yLength = ylen;
            this.zLength = zlen;

            this.root = new Leaf<T>(default(T));
        }

        private void assertBounds(uint x, uint y, uint z)
        {
            if (x >= xLength)
            {
                throw new IndexOutOfRangeException("X position out of bounds, [" + x + "]");
            }
            if (y >= yLength)
            {
                throw new IndexOutOfRangeException("Y position out of bounds, [" + y + "]");
            }
            if (z >= zLength)
            {
                throw new IndexOutOfRangeException("Z position out of bounds, [" + z + "]");
            }
        }

        public T this[uint x, uint y, uint z]
        {
            get
            {
                assertBounds(x, y, z);

                return root.Get(xLength, yLength, zLength, x, y, z);
            }
            set
            {
                assertBounds(x, y, z);

                root = root.Set(xLength, yLength, zLength, x, y, z, value);
            }
        }

        public INode<T> TreeView()
        {
            return new NodeView<T>(root, xLength, yLength, zLength);
        }
    }
}
