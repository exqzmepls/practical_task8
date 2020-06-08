using System;
using System.IO;
using practical_task8;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        // Генератор матриц смежности по заданному эйлеровому пути
        // Генератор выдаст не связный граф, если не указан эйлеров путь
        bool[,] GetMatrix(int n, string eulerPath = "")
        {
            bool[,] result = new bool[n, n];
            if (eulerPath == "")
            {
                Random rnd = new Random();

                // 1 компонента связности
                for (int i = 0; i < n / 2; i++) for (int j = i; j < n / 2; j++) result[i, j] = result[j, i] = rnd.Next(2) == 1;

                // 2 компонента связности
                for (int i = n / 2 + 1; i < n; i++) for (int j = i; j < n; j++) result[i, j] = result[j, i] = rnd.Next(2) == 1;

            }
            else
            {
                string[] pathPoints = eulerPath.Split(new string[] { "->" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < pathPoints.Length - 1; i++)
                {
                    int p1 = int.Parse(pathPoints[i]) - 1;
                    int p2 = int.Parse(pathPoints[i + 1]) - 1;
                    result[p1, p2] = result[p2, p1] = true;
                }
            }
            return result;
        }

        [TestMethod]
        public void TestIntInput()
        {
            Console.SetIn(new StreamReader("intInput.txt"));
            double result = 2;

            double input = Program.IntInput(lBound: 0, uBound: 100, info: "some info");

            Assert.AreEqual(result, input);
        }

        [TestMethod]
        public void TestIsSemiEulerTrue()
        {
            bool result = Program.IsSemiEuler(GetMatrix(8, "1->2->4->8->5->4->6->2->3->5"));
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void TestIsSemiEulerFalseAllEven()
        {
            bool result = Program.IsSemiEuler(GetMatrix(6, "1->2->3->4->5->1"));
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void TestFindEulerPath()
        {
            string result;
            StreamWriter os = new StreamWriter("output.txt", false);
            Console.SetOut(os);
            Program.FindEulerPath(GetMatrix(8, "2->1->4->5->2->6->7->5->8->7->2->3"));
            os.Close();
            using (StreamReader sr = new StreamReader("output.txt"))
            {
                result = sr.ReadLine();
            }
            // Два разых пути в одном графе
            Assert.AreEqual("2->7->8->5->7->6->2->5->4->1->2->3", result);
        }

        [TestMethod]
        public void TestIsSemiEulerFalse()
        {
            bool result = Program.IsSemiEuler(GetMatrix(10));
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void TestIsSemiEulerFalseEmpty()
        {
            bool result = Program.CheckMatrixInput(new string[] { "0 0 0 0 0", "0 0 0 0 0", "0 0 0 0 0", "0 0 0 0 0", "0 0 0 0 0" }, out bool[,] matr);
            result = result && Program.IsSemiEuler(matr);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void TestCheckMatrixInputFalse1()
        {
            bool result = Program.CheckMatrixInput(new string[]{ "1 0 1 0 1", "0 0 1 0 1", "10 10 0", "a b c d e", "-3 4 8 0 1"}, out bool[,] matr);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void TestCheckMatrixInputFalse2()
        {
            bool result = Program.CheckMatrixInput(new string[] { "1 0 1 0 1", "0 0 1 0 1", "1 1 1 0 0", "a b c d e", "-3 4 8 0 1" }, out bool[,] matr);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void TestCheckMatrixInputFalse3()
        {
            bool result = Program.CheckMatrixInput(new string[] { "1 0 1 0 1", "0 0 1 0 1", "1 1 1 0 0", "0 0 0 0 1", "-3 4 8 0 1" }, out bool[,] matr);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void TestCheckMatrixInputFalse4()
        {
            bool result = Program.CheckMatrixInput(new string[] { "1 0 1 0 1", "0 0 1 0 1", "1 1 1 0 0", "0 1 0 0 1", "1 1 0 1 0" }, out bool[,] matr);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void TestCheckMatrixInputTrue()
        {
            bool result = Program.CheckMatrixInput(new string[] { "1 0 1 0 1", "0 0 1 0 1", "1 1 1 0 0", "0 0 0 0 1", "1 1 0 1 0" }, out bool[,] matr);
            Assert.AreEqual(true, result);
        }
    }
}
