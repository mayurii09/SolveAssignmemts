using OnlineOrderProcessing.Models;

namespace OnlineOrderProcessing
{
    internal class Program
    {
        static void Main()
        {
            //Console.WriteLine("Hello, World!");
            List<Order> orders = new List<Order>
            {
                new Order(1,"Mayuri",100.50),
                new Order(2,"Maya",200.40),
                new Order(3,"Radha",300)
            };

            OrderManager orderManager = new OrderManager();
            orderManager.ProcessOrders(orders);

            Console.WriteLine("All orders are processed successfully!!!");

        }
    }
}
