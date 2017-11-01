using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TestFlask.Aspects.Context;
using TestFlask.Aspects.Player;

namespace TestFlask.Aspects.Tests
{
    [TestFixture]
    public class InnerFuncPlayerTests
    {
        private Mock<HttpContextBase> mockHttpContext;

        [SetUp]
        public void Init()
        {
            mockHttpContext = new Mock<HttpContextBase>();

        }

        [Test]
        public void Test_OK()
        {
            HttpContextFactory.Current = mockHttpContext.Object;

            //InnerFuncPlayer funcPlayer = new InnerFuncPlayer<>


        }
    }
}
