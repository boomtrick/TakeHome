using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAllocator
{
    public interface IInventoryDataSource
    {
        void AllocateLine(Product product, int quantity);
        int GetQuantity(Product product);

        int GetTotalInventory();
    }
}
