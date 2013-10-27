namespace Sparse.Array3D
{
    public interface ISparseArray3D<T>
    {
        ISparseArray3D<T> Clone();
        T this[uint x, uint y, uint z] { get; set; }
        INode<T> TreeSnapshot();
    }
}
