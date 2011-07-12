/* Copyright 2011 by Rune Funch Søltoft
 * the code my not be used commercially
 * and when used in any form this copyright notice shall
 * be included in all parts that uses this code
 */
using ClearMudSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClearMudSharp.Traits;
using ClearMudSharp.Tests;

namespace Tests.ClearMudSharp
{
    /// <summary>
    ///This is a test class for ClearMudTest and is intended
    ///to contain all ClearMudTest Unit Tests
    ///</summary>
    [TestClass]
    public class ClearMudTest
    {
        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod]
        public void BindTest()
        {
            var context = MoneyTransferContext.Bind();
            context.Doit(100m);
        }

         /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod]
        public void TraitsMoneyTransferTest()
        {
            var context = new MoneyTransferWithTraits();
            context.Transfer();
        }
        
        [TestMethod]
        public void TestCreate()
        {
            var New = new TraitFactory();
            var add = (ICalc<Add>)New.Create<ITrait>();
            var add2 = (ICalc<Subtract>)add;
            var x1 = 0;
            var x2 = 1;
            var y = 10;
            Assert.AreEqual(y+x1, add.Calculate(x1,y));
            Assert.AreEqual(y + x2, add.Calculate(x2, y));
            Assert.AreEqual(x1 - y, add2.Calculate(x1, y));
            Assert.AreEqual(x2 - y, add2.Calculate(x2, y));
        }
    }

    public class Add
    {
        public static int Calculate(dynamic self, int x, int y)
        {
            return x + y;
        }
    }

    public class Subtract
    {
        public static int Calculate(dynamic self, int x, int y)
        {
            return x - y;
        }
    }

    public interface ITemp : ICalc<Add> { }

    public class Temp : ITemp
    {

        public int Calculate(int x, int y)
        {
            return x + y;
        }
    }

    public class ITrait : Temp, ICalc<Subtract> { }

    [Trait]
    public interface ICalc<T>
    {
        int Calculate(int x, int y);

    }
}