using Mono.Cecil;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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

            newAssemblyPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "AssemblyToProcess.dll");
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

            assembly = Assembly.LoadFrom(newAssemblyPath);
            AppDomain.CurrentDomain.Load(assembly.FullName);
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
        public void Record_BasicScenario1_CreatesSteps_WithDecrementStock()
        {
            //set record mode
            requestHeaders[ContextKeys.TestMode] = TestModes.Record.ToString();

            int customerId = 1;
            int firstProductId = 1;
            int secondProductId = 2;

            //step 1 => get product1 info
            httpItems.Add(ContextKeys.StepNo, 1L);
            dynamic productBiz = assembly.CreateInstance("AssemblyToProcess.Samples.ProductBiz");
            dynamic product1 = productBiz.GetProduct(firstProductId);
            int initialStock1 = (int)product1.Stock;
            httpItems.Clear();

            //step 2 => get product2 info
            httpItems.Add(ContextKeys.StepNo, 2L);
            dynamic product2 = productBiz.GetProduct(secondProductId);
            int initialStock2 = (int)product2.Stock;
            httpItems.Clear();

            //step 3 => find a customer
            httpItems.Add(ContextKeys.StepNo, 3L);
            dynamic customerBiz = assembly.CreateInstance("AssemblyToProcess.Samples.CustomerBiz");
            var customer = customerBiz.GetCustomer(customerId);
            httpItems.Clear();

            //step 4 => create a shopping cart
            httpItems[ContextKeys.StepNo] = 4L;
            dynamic cartBiz = assembly.CreateInstance("AssemblyToProcess.Samples.ShoppingCartBiz");
            var cart = cartBiz.CreateCart(customerId);
            httpItems.Clear();

            //step 5 => add product to cart
            httpItems[ContextKeys.StepNo] = 5L;
            cartBiz.AddToCart(cart, firstProductId);
            httpItems.Clear();

            //step 6 => add another product to cart
            httpItems[ContextKeys.StepNo] = 6L;
            cartBiz.AddToCart(cart, secondProductId);
            httpItems.Clear();

            //assert product stocks are decremented by one.
            Assert.IsTrue(product1.Stock + 1 == initialStock1);
            Assert.IsTrue(product2.Stock + 1 == initialStock2);

            //assert everything is recorded using api in memory collections
            Assert.AreEqual(6, recordedSteps.Count);
        }

        [Test]
        public void Play_BasicScenario1_AddToCartStep_DecrementsStock()
        {
            //first record steps
            Record_BasicScenario1_CreatesSteps_WithDecrementStock();

            //manipulate invocations of recorded steps for an assertion scenario
            recordedSteps.Values.SelectMany(s => s.Invocations).ToList().ForEach(i => i.IsReplayable = false);

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

            int firstProductId = 1;

            //retrigger step 5
            httpItems[ContextKeys.StepNo] = 5L;
            dynamic cart = assembly.CreateInstance("AssemblyToProcess.Samples.ShoppingCart");
            dynamic cartBiz = assembly.CreateInstance("AssemblyToProcess.Samples.ShoppingCartBiz");
            cartBiz.AddToCart(cart, firstProductId);
            httpItems.Clear();

            //Assert that product stock is decremented by one
            Assert.AreEqual(99, cart.Items[0].Stock);
        }

        [Test]
        public void Play_BasicScenario1_ZeroStockManipulation_CausesAppException()
        {
            //first record steps
            Record_BasicScenario1_CreatesSteps_WithDecrementStock();

            //manipulate invocations of recorded steps for an assertion scenario
            recordedSteps.Values.SelectMany(s => s.Invocations).ToList().ForEach(i => i.IsReplayable = false);

            //update all get products for product 1 to return a zero stock, other invocations will not be replayable to actually call original and decrement stock
            recordedSteps.Values.SelectMany(s => s.Invocations)
                .Where(i => i.InvocationSignature.Contains("GetProduct") && i.RequestIdentifierKey == "1")
                .ToList()
                .ForEach(i =>
                {
                    i.IsReplayable = true;
                    i.Response = Regex.Replace(i.Response, "\"Stock\":\\d+","\"Stock\":0");
                });

            //set record mode to play
            httpItems.Clear();
            requestHeaders[ContextKeys.TestMode] = TestModes.Play.ToString();

            int firstProductId = 1;

            //retrigger step 5
            httpItems[ContextKeys.StepNo] = 5L;
            dynamic cart = assembly.CreateInstance("AssemblyToProcess.Samples.ShoppingCart");
            dynamic cartBiz = assembly.CreateInstance("AssemblyToProcess.Samples.ShoppingCartBiz");

            ApplicationException exception = null;

            try
            {
                cartBiz.AddToCart(cart, firstProductId);
            }
            catch (Exception ex)
            {
                exception = ex as ApplicationException;
            }
            
            httpItems.Clear();

            //Assert that product stock is checked and threw valid exception
            Assert.AreEqual("StockNotAvailable", exception.Message);
        }


    }
}
