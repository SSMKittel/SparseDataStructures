namespace Sparse.Array3D
{
    interface INodeInternal<T>
    {
        //The nodes do not store their own length; that must be passed into the methods
        T Get(uint xlen, uint ylen, uint zlen, uint x, uint y, uint z);

        // Return a new tree with {item} set at the specified position
        INodeInternal<T> Set(uint xlen, uint ylen, uint zlen, uint x, uint y, uint z, T item);

        uint LeftLength(uint xlen, uint ylen, uint zlen);
        uint RightLength(uint xlen, uint ylen, uint zlen);

        // The left child node of this tree (null if this node is a leaf)
        INodeInternal<T> Left
        {
            get;
        }

        // The right child node of this tree (null if this node is a leaf)
        INodeInternal<T> Right
        {
            get;
        }

        // Is this node a leaf node?
        bool IsLeaf
        {
            get;
        }

        // The dimension that this node divides (Dimension.None for leaf nodes)
        Dimension Slice
        {
            get;
        }
    }
}
