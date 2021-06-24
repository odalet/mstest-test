using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MSTestTest
{
    [TestClass, ExcludeFromCodeCoverage]
    public class UnitTests
    {
        [TestMethod]
        public void Repro()
        {
            try
            {
                var done = new AutoResetEvent(false);
                done.Dispose();

                var thread = new Thread(() => done.Set());
                thread.Start();
            }
            catch (Exception ex)
            {
                Assert.Fail($"Test failed: {ex}");
            }
        }
    }
}
