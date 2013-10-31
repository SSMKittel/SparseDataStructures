namespace Sparse.Array3D
{
    public interface INode<T>
    {
        INodeInternal<T> Internal
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

        NodeType Type
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

        uint Length(NodeType dimension);

        T this[uint x, uint y, uint z]
        {
            get;
        }
    }
}
