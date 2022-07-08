using InventoryAllocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataSource
{
    public class Datasource
    {
        private InventoryAllocatorFacade Facade;

        private readonly List<string> Products;

        public Datasource()
        {
            this.Products = new List<string>() { "A", "B", "C", "D", "E" };
            Facade = new InventoryAllocatorFacade();
        }

        public void RunOrderStreams()
        {
            var stream1 = new Thread(() => SendOrders());

            var stream2 = new Thread(() => SendOrders());

            var stream3 = new Thread(() => SendOrders());

            stream1.Start();
            stream2.Start();
            stream3.Start();


        }

        private void SendOrders()
        {
            var orderCount = 0;
            var streamId = Guid.NewGuid();

            while(orderCount < 400)
            {
                var order = BuildOrder(orderCount);
                Facade.AddOrder(order, streamId);
                orderCount++;
            }
        }

        private string BuildOrder(int orderCount)
        {
            var order = new StringBuilder("{");
            var header = orderCount + 1;
            order.Append($"\"Header\":{header},\"Lines\":[");
            order.Append(BuildLines());
            order.Append("]}");
            return order.ToString();
        }


        private string BuildLines()
        {
            var lines = new StringBuilder();

            var currentLine = 1;

            foreach (var product in Products)
            {
                var line = new StringBuilder("{\"Product\":");
                line.Append($"\"{product}\", \"Quantity\": \"{RandomNumberGenerator.GetInt32(1, 5)}\"");
                line.Append("}");
                var endString = currentLine == Products.Count ? "" : ",";
                line.Append(endString);
                currentLine++;
                lines.Append(line.ToString());
            }

            return lines.ToString();

        }
    }
}
