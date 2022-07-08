using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAllocator
{
    public interface IInventoryAllocatorService
    {
        void Allocate(Guid streamId, int headerId, Line line);

        List<Line> GetAllocatedLines(Guid streamId, int header);
    }
}
