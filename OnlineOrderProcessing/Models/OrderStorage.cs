using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineOrderProcessing.Models
{
    //thread safe order storage using ConcurrentDictionary
    class OrderStorage
    {
        private ConcurrentDictionary<int, Order> orders = new ConcurrentDictionary<int, Order>();

        public void AddOrder(Order order)
        {
            orders.TryAdd(order.OrderId, order);
        }

        public Order GetOrder(int orderId)
        {
            orders.TryGetValue(orderId, out Order order);
            return order;
        }
    }
}
