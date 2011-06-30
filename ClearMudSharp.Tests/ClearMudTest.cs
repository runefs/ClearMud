using ClearMudSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}