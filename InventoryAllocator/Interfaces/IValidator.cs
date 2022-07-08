using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAllocator
{
    public interface IValidator
    {
        public void ValidateOrder(string input);
    }
}
