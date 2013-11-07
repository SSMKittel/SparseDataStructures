namespace Sparse.Array3D
{
    internal interface IComputedLengthNode<T>
    {
        //The nodes do not store their own length; that must be passed into the methods
        T Get(uint xlen, uint ylen, uint zlen, uint x, uint y, uint z);

        uint LeftLength(uint xlen, uint ylen, uint zlen);
        uint RightLength(uint xlen, uint ylen, uint zlen);

        // The left child node of this tree (null if this node is a leaf)
        IComputedLengthNode<T> Left
        {
            get;
        }

        // The right child node of this tree (null if this node is a leaf)
        IComputedLengthNode<T> Right
        {
            get;
        }

        NodeType Type
        {
            get;
        }
    }
}
