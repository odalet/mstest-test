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
                using var done = new AutoResetEvent(false);
                
                var thread = new Thread(() => 
                { 
                    ok = true; 
                    done.Set(); 
                });

                thread.Start();

                // _ = done.WaitOne(1000);
            }
            catch
            {
                ok = false;
            }

            Assert.IsTrue(ok);
        }
    }
}
