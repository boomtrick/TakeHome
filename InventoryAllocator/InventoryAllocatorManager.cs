using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAllocator
{
    /// <summary>
    ///     dictates order of events based on business logic
    /// </summary>
    internal class InventoryAllocatorManager : IInventoryAllocatorManager
    {
        private readonly IInventoryDataSource _dataSource;

        private readonly IInventoryAllocatorService _inventoryAllocatorService;

        private readonly IBackOrderService _backOrderService;

        private bool ProcessingOrders = false;

        private ConcurrentQueue<(Guid streamId, Order order)> OrderQueue;

        private readonly List<(Guid streamId, Order order)> OrderHistory;

        public InventoryAllocatorManager(IServiceLocator serviceLocator)
        {
            _dataSource = serviceLocator.GetInventoryDataSource();
            _inventoryAllocatorService = serviceLocator.GetInventoryAllocatorService();
            _backOrderService = serviceLocator.GetBackOrderService();
            OrderQueue = new ConcurrentQueue<(Guid streamId, Order order)>();
            OrderHistory = new List<(Guid streamId, Order)>();
        }

        public void AllocateOrder(Guid streamId, Order order)
        {
            if(_dataSource.GetTotalInventory() == 0)
            {
                return;
            }

            OrderQueue.Enqueue((streamId, order));

            if (!ProcessingOrders)
            {
                ProcessingOrders = true;
                ProcessOrders();
            }
        }

        private void ProcessOrders()
        {
            var thread = new Thread(() =>
            {
                var totalInventory = _dataSource.GetTotalInventory();

                while (OrderQueue.Count > 0 && OrderQueue.TryDequeue(out var lineOrder))
                {
                    if (totalInventory == 0)
                    {
                        break;
                    }

                    OrderHistory.Add((lineOrder.streamId, lineOrder.order));
                    AllocateLines(lineOrder.streamId, lineOrder.order.Lines, lineOrder.order.Header, ref totalInventory);
                }

                ProcessingOrders = false;

                if(totalInventory == 0)
                {
                    SendOutput();
                }
            });

            thread.Start();
        }

        private void AllocateLines(
            Guid streamId, 
            List<Line> lines,
            int header,
            ref int totalInventory)
        {
            foreach(var line in lines)
            {
                var currentQuantity = _dataSource.GetQuantity(line.Product);

                if(currentQuantity - line.Quantity >= 0)
                {
                    _dataSource.AllocateLine(line.Product, line.Quantity);
                    _inventoryAllocatorService.Allocate(streamId, header, line);
                    totalInventory -= line.Quantity;
                }
                else
                {
                   _backOrderService.BackOrder(streamId, header, line);
                }
            }
        }

        // since this is a console app we write output to console
        private void SendOutput()
        {
            var output = new StringBuilder();

            foreach(var history in OrderHistory)
            {
                var orderString = new StringBuilder();
                orderString.Append($"{history.order.Header}: ");
                orderString.Append(GetOutputStringForLines(history.order.Lines, "::"));

                var allocatedLines = _inventoryAllocatorService.GetAllocatedLines(history.streamId, history.order.Header);

                orderString.Append(GetOutputStringForLines(allocatedLines, "::"));

                var backOrderedLines = _backOrderService.GetBackOrderedLines(history.streamId, history.order.Header);
                orderString.Append(GetOutputStringForLines(backOrderedLines, ""));

                output.AppendLine(orderString.ToString());
            }

            Console.WriteLine(output.ToString());
        }

        private string GetOutputStringForLines(List<Line> lines, string endString)
        {
            var output = new StringBuilder();

            var allProducts = Enum.GetValues(typeof(Product)).Cast<Product>().ToList();

            for (int i = 0; i < allProducts.Count; i++)
            {
                var line = lines.FirstOrDefault(line => line.Product == allProducts[i]);
                var q = line == null ? 0 : line.Quantity;
                if (i == allProducts.Count - 1)
                {
                    output.Append($"{q}" + endString);
                }
                else
                {
                    output.Append($"{q},");
                }
            }
            return output.ToString();
        }
    }
}
