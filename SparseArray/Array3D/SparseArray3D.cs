using System;

namespace Sparse.Array3D
{
    // Encapsulates the immutable tree structure and presents a mutable object resembling a 3D array
    public class SparseArray3D<T> : ISparseArray3D<T>
    {
        private readonly uint xLength;
        private readonly uint yLength;
        private readonly uint zLength;

        private INodeInternal<T> root;

        // Create a sparse array from an existing 3D array
        public SparseArray3D(T[,,] array)
        {
            this.xLength = (uint) array.GetLength(0);
            this.yLength = (uint) array.GetLength(1);
            this.zLength = (uint) array.GetLength(2);

            this.root = new Leaf<T>(default(T));

            for (uint x = 0; x < xLength; x++)
            {
                for (uint y = 0; y < yLength; y++)
                {
                    for (uint z = 0; z < zLength; z++)
                    {
                        this[x, y, z] = array[x, y, z];
                    }
                }
            }
        }

        // Create a sparse array of the specified dimensions
        public SparseArray3D(uint xlen, uint ylen, uint zlen)
        {
            this.xLength = xlen;
            this.yLength = ylen;
            this.zLength = zlen;

            this.root = new Leaf<T>(default(T));
        }

        // Constructor-based clone method
        public SparseArray3D(SparseArray3D<T> other)
        {
            this.xLength = other.xLength;
            this.yLength = other.yLength;
            this.zLength = other.zLength;
            this.root = other.root;
        }

        public ISparseArray3D<T> Clone()
        {
            return new SparseArray3D<T>(this);
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

        // Get a snapshot view of the tree structure.
        public INode<T> TreeSnapshot()
        {
            return new NodeView<T>(root, xLength, yLength, zLength);
        }
    }
}
