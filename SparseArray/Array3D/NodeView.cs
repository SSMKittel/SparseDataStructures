using System;

namespace Sparse.Array3D
{
    // Provides a read-only view of the internal tree structure, hiding the length whackiness by storing them in the node and creating child nodes on-demand
    class NodeView<T> : INode<T>
    {
        private readonly INodeInternal<T> node;
        private readonly uint xLength;
        private readonly uint yLength;
        private readonly uint zLength;

        public NodeView(INodeInternal<T> node, uint xlen, uint ylen, uint zlen)
        {
            this.node = node;
            this.xLength = xlen;
            this.yLength = ylen;
            this.zLength = zlen;
        }

        public bool IsLeaf
        {
            get
            {
                return node.IsLeaf;
            }
        }

        // Get the left node, if any
        public INode<T> Left
        {
            get
            {
                uint leftLength = node.LeftLength(xLength, yLength, zLength);

                switch (node.Slice)
                {
                    case Dimension.X:
                        return new NodeView<T>(node.Left, leftLength, yLength, zLength);
                    case Dimension.Y:
                        return new NodeView<T>(node.Left, xLength, leftLength, zLength);
                    case Dimension.Z:
                        return new NodeView<T>(node.Left, xLength, yLength, leftLength);
                    default:
                        return null;
                }
            }
        }

        // Get the right node, if any
        public INode<T> Right
        {
            get
            {
                uint rightLength = node.RightLength(xLength, yLength, zLength);

                switch (node.Slice)
                {
                    case Dimension.X:
                        return new NodeView<T>(node.Right, rightLength, yLength, zLength);
                    case Dimension.Y:
                        return new NodeView<T>(node.Right, xLength, rightLength, zLength);
                    case Dimension.Z:
                        return new NodeView<T>(node.Right, xLength, yLength, rightLength);
                    default:
                        return null;
                }
            }
        }

        // The dimension the current node splits on.  Leaf nodes are Dimension.None
        public Dimension Slice
        {
            get
            {
                return node.Slice;
            }
        }

        public uint XLength
        {
            get
            {
                return xLength;
            }
        }

        public uint YLength
        {
            get
            {
                return yLength;
            }
        }

        public uint ZLength
        {
            get
            {
                return zLength;
            }
        }

        // The value stored in this node, if it is a leaf node
        public T Value
        {
            get
            {
                if (node.IsLeaf == false)
                {
                    return default(T);
                }
                else
                {
                    return node.Get(1, 1, 1, 0, 0, 0);
                }
            }
        }

        public uint Length(Dimension dimension)
        {
            switch(dimension)
            {
                case Dimension.X:
                    return this.xLength;
                case Dimension.Y:
                    return this.yLength;
                case Dimension.Z:
                    return this.zLength;
                default:
                    throw new ArgumentException("Not a valid dimension for length: " + dimension);
            }
        }

        public T this[uint x, uint y, uint z]
        {
            get
            {
                assertBounds(x, y, z);
                return node.Get(xLength, yLength, zLength, x, y, z);
            }
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

    }
}
