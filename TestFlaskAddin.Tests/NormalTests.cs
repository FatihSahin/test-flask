using AssemblyToProcess;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
{
    [TestFixture]
    public class NormalTests
    {
        [Test]
        public void TestSomeClient()
        {
            SomeClient client = new SomeClient();
            var response = client.GetSome(new SomeRequest { SomeReqProperty = 55 });
        }
    }
}
