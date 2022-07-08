using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAllocator
{
    /// <summary>
    ///     keeps track of orders and whether they have been allocated or back ordered
    /// </summary>
    internal class InventoryAllocatorService : IInventoryAllocatorService
    {

        //dictionary that tracks allocated lines per order
        //the guid is a combination of the streamid and the header which should create a unique key for this combination
        //we use lists for value because we are simply adding new lines
        private readonly Dictionary<Guid, List<Line>> AllocatedLinesPerOrder;

        //dictionary that tracks the current allocated orders per stream
        //we use dictionary here for value because we are constantly retrieving order keys.
        private readonly Dictionary<Guid, Dictionary<int, Guid>> OrdersPerStream;

        public InventoryAllocatorService()
        {
            AllocatedLinesPerOrder = new Dictionary<Guid, List<Line>>();
            OrdersPerStream = new Dictionary<Guid, Dictionary<int, Guid>>();
        }

        public void Allocate(
            Guid streamId, 
            int header, 
            Line line)
        {
            if (OrdersPerStream.ContainsKey(streamId))
            {
                UpdateAllocationForOrder(streamId, header, line);
            }
            else
            {
                AddNewAllocationForOrder(streamId, header, line);
            }
        }

        public List<Line> GetAllocatedLines(Guid streamId, int header)
        {
            if (OrdersPerStream.ContainsKey(streamId))
            {
                var orders = OrdersPerStream[streamId];
                return AllocatedLinesPerOrder[orders[header]];
            }

            return new List<Line>();
        }

        private void AddNewAllocationForOrder(Guid streamId, int header, Line line)
        {
            var newKey = Guid.NewGuid();
            var orders = new Dictionary<int, Guid>()
                {
                    {header, newKey},
                };
            OrdersPerStream.Add(streamId, orders);
            AllocatedLinesPerOrder.Add(newKey, new List<Line>() { line });
        }

        private void UpdateAllocationForOrder(Guid streamId, int header, Line line)
        {
            var orders = OrdersPerStream[streamId];
            if (orders.ContainsKey(header))
            {
                var allocatedLinesKey = orders[header];
                var lines = AllocatedLinesPerOrder[allocatedLinesKey];
                lines.Add(line);
            }
            else
            {
                var newKey = Guid.NewGuid();
                orders.Add(header, newKey);
                AllocatedLinesPerOrder.Add(newKey, new List<Line> { line });
            }
        }
    }
}
