﻿namespace Sparse.Array3D
{
    interface INodeInternal<T>
    {
        T Get(uint xlen, uint ylen, uint zlen, uint x, uint y, uint z);

        INodeInternal<T> Set(uint xlen, uint ylen, uint zlen, uint x, uint y, uint z, T item);


        uint LeftLength(uint xlen, uint ylen, uint zlen);
        uint RightLength(uint xlen, uint ylen, uint zlen);

        INodeInternal<T> Left
        {
            get;
        }

        INodeInternal<T> Right
        {
            get;
        }
        bool IsLeaf
        {
            get;
        }
        Dimension Slice
        {
            get;
        }
    }
}