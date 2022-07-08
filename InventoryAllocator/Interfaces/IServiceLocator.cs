using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAllocator
{
    public interface IServiceLocator
    {
        IInventoryAllocatorService GetInventoryAllocatorService();

        IInventoryAllocatorManager GetInventoryAllocator();

        IInventoryDataSource GetInventoryDataSource();

        IBackOrderService GetBackOrderService();

        IValidator GetValidator();
    }
}
