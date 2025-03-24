using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineOrderProcessing.Models
{
    //Thread processing
    class OrderManager
    {
        private OrderStorage orderStorage = new OrderStorage();
        private PaymentProcessor paymentProcessor = new PaymentProcessor();
        private object lockObj = new object(); // locking for thread synchronization

        public void ProcessOrders(List<Order> orders)
        {
            List<Thread> threads = new List<Thread>();

            foreach(var order in orders)
            {
                Thread thread = new Thread(() => ProcessOrder(order));
                threads.Add(thread);
                thread.Start();
            }

            foreach(var thread in threads)
            {
                thread.Join(); //Ensure all threads complete their execution
            }
        }

        private void ProcessOrder(Order order)
        {
            lock (lockObj)
            {
                Console.WriteLine($"Processing Order {order.OrderId} for {order.CustomerName}....");
                orderStorage.AddOrder(order);
            }

            Task.Run(async () => await paymentProcessor.ProcessPaymentAsync(order)).Wait();

            Console.WriteLine($"Order {order.OrderId} completed. Paid : {order.IsPaid}");
        }
    }
}
