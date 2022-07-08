using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using InventoryAllocator;
using NUnit;
using NUnit.Framework;

namespace Tests
{
    public class Test
    {
        private readonly TestServiceLocator serviceLocator;

        public Test()
        {
            serviceLocator = new TestServiceLocator();
        }

        [Test]
        public void UseUpAllInventoryTests()
        {
            // Arrange
            var order = "{\"Header\":1, \"Lines\": [{\"Product\":\"B\", \"Quantity\": 5}, {\"Product\":\"A\", \"Quantity\": 1}]}";
            var expectedOutput = "1: 1,5,0,0,0::1,5,0,0,0::0,0,0,0,0";
            var facade = new InventoryAllocatorFacade(serviceLocator);
            // Act
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                var streamId = Guid.NewGuid();
                facade.AddOrder(order, streamId);
                Thread.Sleep(7000);
                var result = sw.ToString().Replace("\r\n","");
                // Assert
                Assert.AreEqual(expectedOutput, result);
            }
        }

        [Test]
        public void BackOrderTest()
        {
            var order = "{\"Header\":1, \"Lines\": [{\"Product\":\"B\", \"Quantity\": 5}, {\"Product\":\"A\", \"Quantity\": 1}, {\"Product\":\"C\", \"Quantity\": 1}]}";
            var expectedOutput = "1: 1,5,1,0,0::1,5,0,0,0::0,0,1,0,0";
            var facade = new InventoryAllocatorFacade(serviceLocator);
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                var streamId = Guid.NewGuid();
                facade.AddOrder(order, streamId);
                Thread.Sleep(7000);
                var result = sw.ToString().Replace("\r\n", "");
                // Assert
                Assert.AreEqual(expectedOutput, result);
            }
        }

        /// <summary>
        ///     should only show orders BEFORE or when inventory reaches 0
        /// </summary>
        [Test]
        public void MultipleOrdersTest()
        {
            var order1 = "{\"Header\":1, \"Lines\": [{\"Product\":\"A\", \"Quantity\": 1}]}";
            var order2 = "{\"Header\":2, \"Lines\": [{\"Product\":\"B\", \"Quantity\": 5}]}";
            var order3 = "{\"Header\":3, \"Lines\": [{\"Product\":\"B\", \"Quantity\": 5}]}";
            var facade = new InventoryAllocatorFacade(serviceLocator);
            var expectedOutput = "1: 1,0,0,0,0::1,0,0,0,0::0,0,0,0,0\r\n2: 0,5,0,0,0::0,5,0,0,0::0,0,0,0,0\r\n\r\n";

            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                var streamId = Guid.NewGuid();
                facade.AddOrder(order1, streamId);
                facade.AddOrder(order2, streamId);
                facade.AddOrder(order3, streamId);
                Thread.Sleep(7000);
                var result = sw.ToString();
                // Assert
                Assert.AreEqual(expectedOutput, result);
            }
        }

        [Test]
        public void MultipleStreamsTest()
        {
            //arrange
            var order1 = "{\"Header\":1, \"Lines\": [{\"Product\":\"A\", \"Quantity\": 1}]}";
            var order2 = "{\"Header\":2, \"Lines\": [{\"Product\":\"B\", \"Quantity\": 5}]}";
            var facade = new InventoryAllocatorFacade(serviceLocator);

            var stream1Id = Guid.NewGuid();
            var stream2Id = Guid.NewGuid();
            var stream1 = new Thread(() => facade.AddOrder(order1, stream1Id));
            var stream2 = new Thread(()=> facade.AddOrder(order2, stream2Id));
            var expectedOutput = "1: 1,0,0,0,0::1,0,0,0,0::0,0,0,0,0\r\n2: 0,5,0,0,0::0,5,0,0,0::0,0,0,0,0\r\n\r\n";

            //act
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                stream1.Start();
                stream2.Start();

                Thread.Sleep(9000);

                var result = sw.ToString();
                // Assert
                Assert.AreEqual(expectedOutput, result);
            }
        }

        [Test]
        public void MultipleStreamsBackorderTest()
        {
            //arrange
            var order1 = "{\"Header\":1, \"Lines\": [{\"Product\":\"A\", \"Quantity\": 1}]}";
            var order2 = "{\"Header\":2, \"Lines\": [{\"Product\":\"A\", \"Quantity\": 1},{\"Product\":\"B\", \"Quantity\": 5}]}";
            var facade = new InventoryAllocatorFacade(serviceLocator);

            var stream1Id = Guid.NewGuid();
            var stream2Id = Guid.NewGuid();
            var stream1 = new Thread(() => facade.AddOrder(order1, stream1Id));
            var stream2 = new Thread(() => facade.AddOrder(order2, stream2Id));
            var expectedOutput = "1: 1,0,0,0,0::1,0,0,0,0::0,0,0,0,0\r\n2: 1,5,0,0,0::0,5,0,0,0::1,0,0,0,0\r\n\r\n";

            //act
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                stream1.Start();
                // insures stream1 gets in first
                Thread.Sleep(1000);
                stream2.Start();

                Thread.Sleep(9000);

                var result = sw.ToString();
                // Assert
                Assert.AreEqual(expectedOutput, result);
            }
        }

        [Test]
        public void MultipleStreamsSameHeaderTest()
        {
            //arrange
            var order1 = "{\"Header\":1, \"Lines\": [{\"Product\":\"A\", \"Quantity\": 1}]}";
            var order2 = "{\"Header\":1, \"Lines\": [{\"Product\":\"B\", \"Quantity\": 5}]}";
            var facade = new InventoryAllocatorFacade(serviceLocator);

            var stream1Id = Guid.NewGuid();
            var stream2Id = Guid.NewGuid();
            var stream1 = new Thread(() => facade.AddOrder(order1,stream1Id));
            var stream2 = new Thread(() => facade.AddOrder(order2, stream2Id));
            var expectedOutput = "1: 1,0,0,0,0::1,0,0,0,0::0,0,0,0,0\r\n1: 0,5,0,0,0::0,5,0,0,0::0,0,0,0,0\r\n\r\n";

            //act
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                stream1.Start();
                Thread.Sleep(2000);
                stream2.Start();

                Thread.Sleep(7000);

                var result = sw.ToString();
                // Assert
                Assert.AreEqual(expectedOutput, result);
            }
        }

        [Test]
        public void SameHeaderSameStreamTest()
        {
            var order1 = "{\"Header\":1, \"Lines\": [{\"Product\":\"A\", \"Quantity\": 1}]}";
            var order2 = "{\"Header\":1, \"Lines\": [{\"Product\":\"B\", \"Quantity\": 5}]}";
            var facade = new InventoryAllocatorFacade(serviceLocator);

            var streamId = Guid.NewGuid();
            var expectedOutput = $"Order not valid.\r\n";

            //act
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                facade.AddOrder(order1, streamId);
                facade.AddOrder(order2, streamId);

                Thread.Sleep(9000);

                var result = sw.ToString();
                // Assert
                Assert.AreEqual(expectedOutput, result);
            }
        }

        [Test]
        public void OrderContainsInvalidProductTest()
        {
            var order1 = "{\"Header\":1, \"Lines\": [{\"Product\":\"Z\", \"Quantity\": 1}]}";
            var facade = new InventoryAllocatorFacade(serviceLocator);

            var streamId = Guid.NewGuid();
            var expectedOutput = $"Order not valid.\r\n";

            //act
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                facade.AddOrder(order1, streamId);

                Thread.Sleep(7000);

                var result = sw.ToString();
                // Assert
                Assert.AreEqual(expectedOutput, result);
            }
        }

        [Test]
        public void OrderContainsNoLinesTest()
        {
            var order1 = "{\"Header\":1, \"Lines\": []}";
            var facade = new InventoryAllocatorFacade(serviceLocator);

            var streamId = Guid.NewGuid();
            var expectedOutput = $"Order not valid.\r\n";

            //act
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                facade.AddOrder(order1, streamId);

                Thread.Sleep(7000);

                var result = sw.ToString();
                // Assert
                Assert.AreEqual(expectedOutput, result);
            }
        }

        [Test]
        public void OrderInvalidStringFormatTest()
        {
            var order1 = "Hello World";
            var facade = new InventoryAllocatorFacade(serviceLocator);

            var streamId = Guid.NewGuid();
            var expectedOutput = $"Order not valid.\r\n";

            //act
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                facade.AddOrder(order1, streamId);

                Thread.Sleep(7000);

                var result = sw.ToString();
                // Assert
                Assert.AreEqual(expectedOutput, result);
            }
        }

        [Test]
        public void LineNegativeQuantityTest()
        {
            var order = "{\"Header\":1, \"Lines\": [{\"Product\":\"B\", \"Quantity\": 5}, {\"Product\":\"A\", \"Quantity\": -1}, {\"Product\":\"C\", \"Quantity\": 1}]}";
            var expectedOutput = $"Order not valid.\r\n";
            var facade = new InventoryAllocatorFacade(serviceLocator);
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                var streamId = Guid.NewGuid();
                facade.AddOrder(order, streamId);
                Thread.Sleep(7000);
                var result = sw.ToString();
                // Assert
                Assert.AreEqual(expectedOutput, result);
            }
        }

        [Test]
        public void LineQuantityOverFiveTest()
        {
            var order = "{\"Header\":1, \"Lines\": [{\"Product\":\"B\", \"Quantity\": 6}, {\"Product\":\"A\", \"Quantity\": 1}, {\"Product\":\"C\", \"Quantity\": 1}]}";
            var expectedOutput = $"Order not valid.\r\n";
            var facade = new InventoryAllocatorFacade(serviceLocator);
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                var streamId = Guid.NewGuid();
                facade.AddOrder(order, streamId);
                Thread.Sleep(7000);
                var result = sw.ToString();
                // Assert
                Assert.AreEqual(expectedOutput, result);
            }
        }
    }
}
