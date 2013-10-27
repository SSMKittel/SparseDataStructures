namespace Sparse.Array3D
{
    public interface INode<T>
    {
        bool IsLeaf
        {
            get;
        }

        INode<T> Left
        {
            get;
        }

        INode<T> Right
        {
            get;
        }

        Dimension Slice
        {
            get;
        }

        uint XLength
        {
            get;
        }

        uint YLength
        {
            get;
        }

        uint ZLength
        {
            get;
        }

        T Value
        {
            get;
        }

        uint Length(Dimension dimension);

        T this[uint x, uint y, uint z]
        {
            get;
        }
    }
}
