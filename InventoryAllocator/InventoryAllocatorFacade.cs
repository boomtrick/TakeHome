using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Tests")]

namespace InventoryAllocator
{
    /// <summary>
    ///     The entry way to the system
    /// </summary>
    public class InventoryAllocatorFacade
    {
        private readonly IInventoryAllocatorManager _inventoryAllocator;

        private readonly IValidator _validator;

        public InventoryAllocatorFacade()
        {
            // would normally use a DI framework but not built in for console apps
            // using  a service locator instead which accomplishes dependency inversion
            var serviceLocator = new ServiceLocator();
            _inventoryAllocator = serviceLocator.GetInventoryAllocator();
            _validator = serviceLocator.GetValidator();
        }


        //mainly for testing purposes
        public InventoryAllocatorFacade(IServiceLocator serviceLocator)
        {
            _inventoryAllocator = serviceLocator.GetInventoryAllocator();
            _validator = serviceLocator.GetValidator();
        }

        public void AddOrder(string order, Guid streamId)
        {
            try
            {
                _validator.ValidateOrder(order);
                var orderAsDto = JsonConvert.DeserializeObject<Order>(order);
                _inventoryAllocator.AllocateOrder(streamId, orderAsDto);
            }
            catch (Exception ex)
            {
                ManageExceptions(ex);
            }
        }

        // handle errors. log them. throw consumer facing message. etc
        private void ManageExceptions(Exception ex)
        {
            if(ex is ValidationException)
            {
                Console.WriteLine($"Order not valid.");
            }
            else
            {
                Console.WriteLine("Internal error");
            }

        }
    }
}
