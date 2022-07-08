using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace InventoryAllocator
{
    public class Line
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public Product Product { get; set; }

        public int Quantity { get; set; }
    }
}
