using System.Collections.Generic;
using System;

namespace Sparse.Array3D
{
    public interface INodeFactory<T>
    {
        INode<T> Get(T item, uint xlen, uint ylen, uint zlen);
        INode<T> Get(Dimension dim, INode<T> left, INode<T> right);
        INode<T> WithSet(INode<T> node, uint x, uint y, uint z, T item);
    }
}
