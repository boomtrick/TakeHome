using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace InventoryAllocator
{
    internal class Validator : IValidator
    {
        public void ValidateOrder(string order)
        {
            try
            {
                var orderAsDto = JsonConvert.DeserializeObject<Order>(order);

                var quantityErrorMessage = new StringBuilder();
                foreach (var line in orderAsDto.Lines)
                {
                    if (line.Quantity > 5 || line.Quantity < 1)
                    {
                        var error = $"Quantity of {line.Quantity} for Product: {line.Product} for Header{orderAsDto.Header} contains invalid quantity";
                        quantityErrorMessage.AppendLine(error);
                    }
                }

                if(quantityErrorMessage.Length > 0)
                {
                    throw new Exception(quantityErrorMessage.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new ValidationException(ex.Message);
            }
            
        }
    }
}
