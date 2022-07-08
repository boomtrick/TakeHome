using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAllocator
{
    public class Order
    {
        public int Header { get; set; }

        public List<Line> Lines { get; set; }
    }
}
