using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineOrderProcessing.Models
{
    //Async Processing
    class PaymentProcessor 
    {
        public async Task<bool> ProcessPaymentAsync(Order order)
        {
            Console.WriteLine($"Processing payment for Order {order.OrderId}....");
            await Task.Delay(2000); //payment delay
            order.IsPaid = true;
            Console.WriteLine($"Payment successful for Order {order.OrderId}.");
            return true;
        }
    }
}
