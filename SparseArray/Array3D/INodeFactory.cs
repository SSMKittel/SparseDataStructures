using System.Collections.Generic;
using System;

namespace Sparse.Array3D
{
    public interface INodeFactory<T>
    {
        INodeInternal<T> Get(T item);
        INodeInternal<T> Get(Dimension dim, INodeInternal<T> left, INodeInternal<T> right);
        INodeInternal<T> WithSet(INodeInternal<T> node, uint xlen, uint ylen, uint zlen, uint x, uint y, uint z, T item);
    }
}
