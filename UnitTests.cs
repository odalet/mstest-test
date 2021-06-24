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
