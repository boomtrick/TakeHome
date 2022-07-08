using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAllocator
{
    internal class ServiceLocator : IServiceLocator
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
            return new InventoryDataSource();
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
