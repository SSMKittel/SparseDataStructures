using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sparse.Array3D;

namespace Test
{
    [TestClass]
    public class SparseArray3DTest
    {
        private readonly uint CubeLength = 10;
        private int[, ,] UniqueCube
        {
            get
            {
                int i = 1;
                int[, ,] vals = new int[CubeLength, CubeLength, CubeLength];

                for (int x = 0; x < CubeLength; x++)
                {
                    for (int y = 0; y < CubeLength; y++)
                    {
                        for (int z = 0; z < CubeLength; z++)
                        {
                            vals[x, y, z] = i;
                            i++;
                        }
                    }
                }

                return vals;
            }
        }

        private int[, ,] IdenticalCube(int value)
        {
            int[, ,] vals = new int[CubeLength, CubeLength, CubeLength];

            for (int x = 0; x < CubeLength; x++)
            {
                for (int y = 0; y < CubeLength; y++)
                {
                    for (int z = 0; z < CubeLength; z++)
                    {
                        vals[x, y, z] = value;
                    }
                }
            }

            return vals;
        }

        [TestMethod]
        public void UniqueValuesCube()
        {
            var cube = UniqueCube;

            SparseArray3D<int> arr = new SparseArray3D<int>(cube);

            AssertEqual(cube, arr);
        }

        [TestMethod]
        public void IdenticalValuesCube()
        {
            var cube = IdenticalCube(123);

            SparseArray3D<int> arr = new SparseArray3D<int>(cube);

            AssertEqual(cube, arr);

            var snap = arr.TreeSnapshot();
            Assert.IsTrue(snap.IsLeaf, "is leaf");
        }

        [TestMethod]
        public void ChangingValuesCube()
        {
            SparseArray3D<int> arr = new SparseArray3D<int>(CubeLength, CubeLength, CubeLength);

            var uniqueCube = UniqueCube;
            SetAll(uniqueCube, arr);
            AssertEqual(uniqueCube, arr);

            var identCube = IdenticalCube(123);
            SetAll(identCube, arr);
            AssertEqual(identCube, arr);

            var snap = arr.TreeSnapshot();
            Assert.IsTrue(snap.IsLeaf, "is leaf");
        }

        private void SetAll(int[, ,] values, SparseArray3D<int> arr)
        {
            for (uint x = 0; x < values.GetLength(0); x++)
            {
                for (uint y = 0; y < values.GetLength(1); y++)
                {
                    for (uint z = 0; z < values.GetLength(2); z++)
                    {
                        arr[x, y, z] = values[x, y, z];
                    }
                }
            }
        }

        private void AssertEqual(int[, ,] cube, SparseArray3D<int> arr)
        {
            for (uint x = 0; x < cube.GetLength(0); x++)
            {
                for (uint y = 0; y < cube.GetLength(1); y++)
                {
                    for (uint z = 0; z < cube.GetLength(2); z++)
                    {
                        string pos = String.Format("[{0}, {1}, {2}]", x, y, z);

                        Assert.AreEqual(cube[x, y, z], arr[x, y, z], pos);
                    }
                }
            }
        }
    }
}
