using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAllocator
{
    public interface IBackOrderService
    {
        void BackOrder(Guid streamId, int header, Line line);

        List<Line> GetBackOrderedLines(Guid streamId, int header);
    }
}
