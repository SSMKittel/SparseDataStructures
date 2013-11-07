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

        private readonly INodeFactory<T> nodeFactory;

        // Create a sparse array from an existing 3D array
        public SparseArray3D(INodeFactory<T> factory, T[, ,] array)
            : this(
                factory,
                (uint)array.GetLength(0),
                (uint)array.GetLength(1),
                (uint)array.GetLength(2))
        {
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

        public SparseArray3D(T[, ,] array) : this(new SimpleFactory<T>(), array) { }

        public SparseArray3D(uint xlen, uint ylen, uint zlen)
            : this(new SimpleFactory<T>(), xlen, ylen, zlen)
        { }

        // Create a sparse array of the specified dimensions
        public SparseArray3D(INodeFactory<T> factory, uint xlen, uint ylen, uint zlen)
        {
            if (xlen == 0)
            {
                throw new ArgumentException("X dimension must be greater than zero");
            }

            if (ylen == 0)
            {
                throw new ArgumentException("Y dimension must be greater than zero");
            }

            if (zlen == 0)
            {
                throw new ArgumentException("Z dimension must be greater than zero");
            }

            this.xLength = xlen;
            this.yLength = ylen;
            this.zLength = zlen;

            this.nodeFactory = factory;

            this.root = this.nodeFactory.Get(default(T));
        }

        // Constructor-based clone method
        public SparseArray3D(SparseArray3D<T> other)
        {
            this.xLength = other.xLength;
            this.yLength = other.yLength;
            this.zLength = other.zLength;
            this.root = other.root;
            this.nodeFactory = other.nodeFactory;
        }

        public ISparseArray3D<T> Clone()
        {
            return new SparseArray3D<T>(this);
        }

        public void Fill(T item)
        {
            this.root = this.nodeFactory.Get(item);
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

                root = nodeFactory.WithSet(root, xLength, yLength, zLength, x, y, z, value);
            }
        }

        // Get a snapshot view of the tree structure.
        public INode<T> TreeSnapshot()
        {
            return new NodeView<T>(root, xLength, yLength, zLength);
        }
    }
}
