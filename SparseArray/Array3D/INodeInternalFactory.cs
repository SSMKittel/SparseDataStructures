using System.Collections.Generic;
using System;

namespace Sparse.Array3D
{
    public interface INodeInternalFactory<T>
    {
        INodeInternal<T> Get(T item);
        INodeInternal<T> Get(NodeType dim, INodeInternal<T> left, INodeInternal<T> right);
    }
}
