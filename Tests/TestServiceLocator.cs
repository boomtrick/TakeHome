using InventoryAllocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Tests
{
    internal class TestServiceLocator : IServiceLocator
    {
        public IInventoryAllocatorService GetInventoryAllocatorService()
        {
            return new InventoryAllocatorService();
        }

        public IInventoryAllocatorManager GetInventoryAllocator()
        {
            return new InventoryAllocatorManager(this);
        }

        public IInventoryDataSource GetInventoryDataSource()
        {
            var lines = new List<Line>()
            {
                new Line(){Product = Product.A, Quantity = 1},
                new Line(){Product = Product.B, Quantity = 5}
            };

            return new InventoryDataSource(lines);
        }

        public IBackOrderService GetBackOrderService()
        {
            return new BackOrderService();
        }

        public IValidator GetValidator()
        {
            return new Validator();
        }
    }
}
