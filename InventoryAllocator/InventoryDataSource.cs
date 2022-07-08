using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAllocator
{
    /// <summary>
    ///     data store that keeps track of current inventory
    /// </summary>
    internal class InventoryDataSource : IInventoryDataSource
    {
        private readonly Dictionary<Product, int> _inventory;

        public InventoryDataSource()
        {
            _inventory = new Dictionary<Product, int>()
            {
                { Product.A, 150},
                { Product.B, 150 },
                { Product.C, 100 },
                { Product.D, 100 },
                { Product.E, 100 }
            };
        }

        /// mainly for testing
        public InventoryDataSource(List<Line> inventory)
        {
            _inventory = new Dictionary<Product, int>();

            foreach (Line line in inventory)
            {
                _inventory.Add(line.Product, line.Quantity);
            }
        }

        public void AllocateLine(Product product, int quantity)
        { 
            _inventory[product] = _inventory[product] - quantity;
        }

        public int GetQuantity(Product product)
        {
            if (_inventory.ContainsKey(product))
            {
                return _inventory[product];
            }

            return 0;
        }

        public int GetTotalInventory()
        {
            var quantity = 0;
            foreach(var quant in _inventory.Values)
            {
                quantity += quant;
            }

            return quantity;
        }

    }
}
