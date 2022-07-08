using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAllocator
{
    //tracks backorders for each product per order
    internal class BackOrderService : IBackOrderService
    {
        private readonly Dictionary<Guid, List<Line>> BackOrderedLinesPerOrder;

        private readonly Dictionary<Guid, Dictionary<int, Guid>> OrdersPerStream;

        public BackOrderService()
        {
            BackOrderedLinesPerOrder = new Dictionary<Guid, List<Line>>();
            OrdersPerStream = new Dictionary<Guid, Dictionary<int, Guid>>();
        }

        public void BackOrder(Guid streamId, int header, Line line)
        {
            if (OrdersPerStream.ContainsKey(streamId))
            {
                UpdateBackOrderedLinesForOrder(streamId, header, line);
            }
            else
            {
                AddNewBackOrderForOrder(streamId, header, line);
            }
        }


        public List<Line> GetBackOrderedLines(Guid streamId, int header)
        {
            if (OrdersPerStream.ContainsKey(streamId))
            {
                var orders = OrdersPerStream[streamId];
                return BackOrderedLinesPerOrder[orders[header]];
            }

            return new List<Line>();
        }

        private void AddNewBackOrderForOrder(Guid streamId, int header, Line line)
        {
            var newKey = Guid.NewGuid();
            var orders = new Dictionary<int, Guid>()
                {
                    {header, newKey},
                };
            OrdersPerStream.Add(streamId, orders);
            BackOrderedLinesPerOrder.Add(newKey, new List<Line>() { line });
        }

        private void UpdateBackOrderedLinesForOrder(Guid streamId, int header, Line line)
        {
            var orders = OrdersPerStream[streamId];
            if (orders.ContainsKey(header))
            {
                var allocatedLinesKey = orders[header];
                var lines = BackOrderedLinesPerOrder[allocatedLinesKey];
                lines.Add(line);
            }
            else
            {
                var newKey = Guid.NewGuid();
                orders.Add(header, newKey);
                BackOrderedLinesPerOrder.Add(newKey, new List<Line> { line });
            }
        }
    }
}
