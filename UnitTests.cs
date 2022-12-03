using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MSTestTest
{
    [TestClass, ExcludeFromCodeCoverage]
    public class UnitTests
    {
        // Suggested by https://github.com/microsoft/testfx/issues/887#issuecomment-1332424036
        [TestMethod]
        public void Repro()
        {
            var ok = false;
            var threadLambdaWasCalled = false;
            try
            {
                var thread = new Thread(() =>
                {
                    ok = true;
                    threadLambdaWasCalled = true;
                    throw new Exception("TEST");
                });

                thread.Start();
            }
            catch
            {
                ok = false;
                Assert.Fail("Reached catch");
            }

            while (!threadLambdaWasCalled)
                Thread.Sleep(100);

            Thread.Sleep(100);
            Assert.IsTrue(ok);
        }

        // Original flawed repro
        //[TestMethod]
        public void Repro_flawed()
        {
            var ok = false;
            try
            {
                var thread = new Thread(() =>
                {
                    ok = true;
                    throw new Exception("TEST");
                });

                thread.Start();
            }
            catch
            {
                ok = false;
            }

            Assert.IsTrue(ok);
        }
    }
}