using System.Runtime.CompilerServices;

namespace VehicleManagementSystem
{
    abstract class Vehicle //abstarction
    {
        public string Model { get; set; } //Encapsulation
        public int Year { get; set; }

        public Vehicle(string model, int year)
        {
            Model = model;
            Year = year;
        }

        public override string ToString()
        {
            return "Vehicle: " + Model + "," + Year;
        }

        public abstract void Drive();

        public void Display<T>(T vehicle)
        {
            Console.WriteLine(vehicle.ToString());
        }
    }

    class Car : Vehicle //inheritance
    {
        public string CarType { get; set; }

        public Car(string model, int year, string carType) : base(model, year)
        {
            CarType = carType;
        }

        public override string ToString()
        {
            return "Car: " + Model + "," + Year + "," + CarType;
        }

        public override void Drive()
        {
            Console.WriteLine($"Driving a {CarType} car - {Model} , {Year}");
        }
    }


    class Truck : Vehicle
    {
        public int CargoCapacity { get; set; }

        public Truck(string model, int year, int cargoCapacity) : base(model, year)
        {
            CargoCapacity = cargoCapacity;
        }

        public override string ToString()
        {
            return "Truck: " + Model + "," + Year + "," + CargoCapacity;
        }

        public override void Drive()
        {
            Console.WriteLine($"Driving a Truck - model = {Model}, year = {Year}, Truck has {CargoCapacity} tons of Cargo Capacity");
        }
    }


    class MotorCycle : Vehicle
    {
        public bool HasDiskBreak { get; set; }

        public MotorCycle(string model, int year, bool hasDiskBreak) : base(model, year)
        {
            HasDiskBreak = hasDiskBreak;
        }

        public override string ToString()
        {
            return "MotorCycle: " + Model + "," + Year + "," + HasDiskBreak;
        }

        public override void Drive()
        {
            Console.WriteLine($"Driving a MotorCycle - model = {Model}, year = {Year}, Disk Break = {HasDiskBreak}");
        }
    }

    static class AnotherClass
    {
        public static void Start(this Vehicle v) //Extension methods
        {
            Console.WriteLine("Vehicle Started!!!");
        }

        public static void Stop(this Vehicle v)
        {
            Console.WriteLine("Vehicle Stopped!!!");
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello, World!");


            //OOPS Concepts, generics, and Extension methods implementation
            //Vehicle vehicle;

            //Console.WriteLine("Select a vehicle to drive:");
            //Console.WriteLine("1. Car");
            //Console.WriteLine("2. Truck");
            //Console.WriteLine("3. MotorCycle");
            //Console.Write("Enter your Choice(1-3): ");
            //string choice = Console.ReadLine();

            //switch (choice)
            //{
            //    case "1":
            //        vehicle = new Car("XUV-700", 2016, "Mahindra");
            //        break;
            //    case "2":
            //        vehicle = new Truck("Ford", 2020, 3);
            //        break;
            //    case "3":
            //        vehicle = new MotorCycle("Bullet", 2017, true);
            //        break;
            //    default:
            //        Console.WriteLine("Invalid Choice.");
            //        return;
            //}

            //Console.WriteLine($"Selected Vehicle: {vehicle.GetType().Name} - {vehicle.Model} ({vehicle.Year})");
            //vehicle.Start();
            //vehicle.Drive(); //polymorphism
            //vehicle.Stop();
            //vehicle.Display(vehicle);

            Console.WriteLine("\nLINQ implementation: ");
            List<Vehicle> vehicles = new List<Vehicle>
            {
                new Car("Toyoto",2020,"Diesel"),
                new Truck("Ford",2018,3),
                new MotorCycle("Bullet",2022,true),
                new Car("Honda",2019,"Petrol"),
                new Truck("Mahindra",2021,4),
                new MotorCycle("MT-15",2021,true)
            };

            //LINQ
            var cars = vehicles.OfType<Car>().Where(v => v.Year > 2018).ToList();
            var trucks = vehicles.OfType<Truck>().Where(v => v.Year <= 2020).ToList();
            var motorCycles = vehicles.OfType<MotorCycle>().Where(v => v.Year == 2021).ToList();

            //Displaying data
            Console.WriteLine("Cars (Year > 2018): ");
            foreach(var car in cars)
            {
                Console.WriteLine($"Model: {car.Model}, Year: {car.Year}, Car Type: {car.CarType}");
            }

            Console.WriteLine("Truck (Year <= 2020): ");
            foreach(var truck in trucks)
            {
                Console.WriteLine($"Model: {truck.Model}, Year: {truck.Year}, Cargo Capacity: {truck.CargoCapacity}");
            }

            Console.WriteLine("MotorCycle (Year == 2021): "); 
            foreach(var motorcycle in motorCycles)
            {
                Console.WriteLine($"Model: {motorcycle.Model}, Year: {motorcycle.Year}, Has Disk Break: {motorcycle.HasDiskBreak}");
            }

            Console.WriteLine("\nGrouping of vehicles based on Year: ");
            var groupByYear = vehicles.GroupBy(v => v.Year);
            foreach(var group in groupByYear)
            {
                Console.WriteLine($"Year: {group.Key}");
                foreach(var vehicle in group)
                {
                    Console.WriteLine($" {vehicle}");
                }
            }

            var orderByModel = vehicles.OrderBy(v => v.Model).ToList(); //Early execution
            Console.WriteLine("\nOrdering of vehicles based on Model: ");
            foreach(var vehicle in orderByModel)
            {
                Console.WriteLine(vehicle);
            }

        }
    }
}
