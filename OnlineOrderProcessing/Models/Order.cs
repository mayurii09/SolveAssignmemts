using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineOrderProcessing.Models
{
    //representing order
    class Order
    {
        public int OrderId { get; }

        public string CustomerName { get; }

        public double Amount { get; }
        
        public bool IsPaid { get; set; }

        public Order(int orderId, string customerName, double amount)
        {
            OrderId = orderId;
            CustomerName = customerName;
            Amount = amount;
            IsPaid = false;
        }


    }
}
