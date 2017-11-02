using AssemblyToProcess.Samples;
using Mono.Cecil;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestFlask.Aspects.Context;
using TestFlask.Aspects.Enums;
using TestFlask.Models.Context;

namespace TestFlask.Aspects.Tests.WeaveTests
{
    [TestFixture]
    public class SampleScenarioTests : PlayerTestsBase
    {
        private Assembly assembly;
        private string newAssemblyPath;
        private string assemblyPath;


        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var projectPath = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\AssemblyToProcess\AssemblyToProcess.csproj"));
            assemblyPath = Path.Combine(Path.GetDirectoryName(projectPath), @"bin\Debug\AssemblyToProcess.dll");

            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(Path.Combine(Path.GetDirectoryName(projectPath), @"bin\Debug\"));

#if (!DEBUG)
            assemblyPath = assemblyPath.Replace("Debug", "Release");
#endif

            newAssemblyPath = assemblyPath.Replace(".dll", ".AspectsTests.dll");
            File.Copy(assemblyPath, newAssemblyPath, true);

            using (var moduleDefinition = ModuleDefinition.ReadModule(assemblyPath, new ReaderParameters { AssemblyResolver = resolver }))
            {
                var weavingTask = new ModuleWeaver
                {
                    ModuleDefinition = moduleDefinition
                };

                weavingTask.Execute();
                moduleDefinition.Write(newAssemblyPath);
            }

            assembly = Assembly.LoadFile(newAssemblyPath);
        }

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
        }

        [TearDown]
        protected override void TearDown()
        {
            base.TearDown();

        }


        [Test]
        public void Record_BasicScenario1_Steps()
        {
            //set record mode
            requestHeaders[ContextKeys.TestMode] = TestModes.Record.ToString();

            int customerId = 1;
            int firstProductId = 1;
            int secondProductId = 2;

            int initialStock1, initialStock2, finalStock1, finalStock2;

            TriggerScenario1(customerId, firstProductId, secondProductId, out initialStock1, out initialStock2, out finalStock1, out finalStock2);

            Assert.IsTrue(finalStock1 + 1 == initialStock1);
            Assert.IsTrue(finalStock2 + 1 == initialStock2);

            //assert everything is recorded using api in memory collections
            Assert.AreEqual(6, recordedSteps.Count);
        }

        private void TriggerScenario1(int customerId, int firstProductId, int secondProductId, out int initialStock1, out int initialStock2, out int finalStock1, out int finalStock2)
        {
            //step 1 => get product1 info
            httpItems.Add(ContextKeys.StepNo, 1L);
            var productBiz = assembly.CreateInstance("AssemblyToProcess.Samples.ProductBiz");
            var product1 = productBiz.GetType().GetMethod("GetProduct").Invoke(productBiz, new object[] { firstProductId });
            initialStock1 = (int)product1.GetType().GetProperty("Stock").GetValue(product1);
            httpItems.Clear();

            //step 2 => get product2 info
            httpItems.Add(ContextKeys.StepNo, 2L);
            var product2 = productBiz.GetType().GetMethod("GetProduct").Invoke(productBiz, new object[] { secondProductId });
            initialStock2 = (int)product2.GetType().GetProperty("Stock").GetValue(product2);
            httpItems.Clear();

            //step 3 => find a customer
            httpItems.Add(ContextKeys.StepNo, 3L);
            var customerBiz = assembly.CreateInstance("AssemblyToProcess.Samples.CustomerBiz");
            var customer = customerBiz.GetType().GetMethod("GetCustomer").Invoke(customerBiz, new object[] { customerId });
            httpItems.Clear();

            //step 4 => create a shopping cart
            httpItems[ContextKeys.StepNo] = 4L;
            var cartBiz = assembly.CreateInstance("AssemblyToProcess.Samples.ShoppingCartBiz");
            var cart = cartBiz.GetType().GetMethod("CreateCart").Invoke(cartBiz, new object[] { customerId });
            httpItems.Clear();

            //step 5 => add product to cart
            httpItems[ContextKeys.StepNo] = 5L;
            var addToCartMethod = cartBiz.GetType().GetMethod("AddToCart");
            addToCartMethod.Invoke(cartBiz, new object[] { cart, firstProductId });
            httpItems.Clear();

            //step 6 => add another product to cart
            httpItems[ContextKeys.StepNo] = 6L;
            addToCartMethod.Invoke(cartBiz, new object[] { cart, secondProductId });
            httpItems.Clear();

            //assert product stocks are decremented by one.
            finalStock1 = (int)product1.GetType().GetProperty("Stock").GetValue(product1);
            finalStock2 = (int)product1.GetType().GetProperty("Stock").GetValue(product2);
        }

        [Test]
        [Ignore("Cannot load types from renamed assembly yet.")]
        public void Play_BasicScenario1_Steps()
        {
            //first record steps
            Record_BasicScenario1_Steps();

            //manipulate invocations of recorded steps for an assertion scenario
            //update all get products for product 1 to return a 100 stock, other invocations will not be replayable to actually call original and decrement stock
            recordedSteps.Values.SelectMany(s => s.Invocations)
                .Where(i => i.InvocationSignature.Contains("GetProduct") && i.RequestIdentifierKey == "1")
                .ToList()
                .ForEach(i =>
                {
                    i.IsReplayable = true;
                    i.Response = i.Response.Replace("\"Stock\":10", "\"Stock\":100");
                });

            //set record mode to play
            httpItems.Clear();
            requestHeaders[ContextKeys.TestMode] = TestModes.Play.ToString();

            int customerId = 1;
            int firstProductId = 1;
            int secondProductId = 2;

            int initialStock1, initialStock2, finalStock1, finalStock2;
            //retrigger scenario
            TriggerScenario1(customerId, firstProductId, secondProductId, out initialStock1, out initialStock2, out finalStock1, out finalStock2);

            //assert
            Assert.AreEqual(100, initialStock1);
            Assert.AreEqual(99, finalStock1);
        }
    }
}
