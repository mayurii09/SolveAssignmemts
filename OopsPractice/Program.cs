namespace OopsPractice
{
    internal class Program
    {
        class Person
        {
            public String FirstName { get; set; }
            public String LastName { get; set; }

            public Person(String FirstName, String LastName)
            {
                this.FirstName = FirstName;
                this.LastName = LastName;
            }

            public virtual void Greetings()
            {
                Console.WriteLine("Greetings from Person");
            }
        }
        class Employee : Person
        {
            public int EmployeeId { get; set; }

            public Employee(int EmployeeId, String FirstName, String LastName) : base(FirstName, LastName)
            {
                this.EmployeeId = EmployeeId;
            }

            public override void Greetings()
            {
                Console.WriteLine("Greetings from Employee");
            }

        }
        static void Main(string[] args)
        {
            Console.WriteLine("Person Object");
            Person p = new Person("Maya", "Patil");
            p.Greetings();


            Console.WriteLine();
            Console.WriteLine("Employee Object");
            Person pObj = new Employee(1, "Mayuri", "Aher");
            pObj.Greetings();

        }
    }
}
