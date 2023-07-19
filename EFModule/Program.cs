using System.Data.Entity;
using System.Xml.Linq;

namespace EFModule
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<CarDb>());
            InsertData();
            QueryData();
            
        }

        private static void QueryData()
        {
            var db = new CarDb();
            //Show the database queries in Logs
            //db.Database.Log = Console.WriteLine;

            //Quey syntax
            var query = from car in db.Cars
                        orderby car.Combined descending, car.Name ascending
                        select car;

            //Method syntax
            var query2 =
                db.Cars.Where(c => c.Manufacturer == "BMW")
                       .OrderByDescending(c => c.Combined)
                       .ThenBy(c => c.Name).Take(10);

            //foreach (var car in query)
            //{
            //    Console.WriteLine($"{car.Name}: {car.Combined}");
            //}

            //Advanced LINQ Query
            //Method syntax
            var query3 =
                db.Cars.GroupBy(c => c.Manufacturer)
                        .Select(g => new
                        {
                            Name = g.Key,
                            Cars = g.OrderByDescending(c =>c.Combined).Take(2)
                        });

            //Query syntax
            var query4 =
                from car in db.Cars
                group car by car.Manufacturer into manufacturer
                select new
                {
                    Name = manufacturer.Key,
                    Cars = (from car in manufacturer
                           orderby car.Combined descending
                           select car).Take(2)
                };

            foreach (var group in query3)
            {
                Console.WriteLine(group.Name);
                foreach (var car in group.Cars)
                {
                    Console.WriteLine($"\t{car.Name}: {car.Combined}");
                }
            }
        }

        private static void InsertData()
        {
            var cars = ProcessCars("fuel.csv");
            var db = new CarDb();
            db.Database.Log = Console.WriteLine;

            if (!db.Cars.Any())
            {
                foreach (var car in cars)
                {
                    db.Cars.Add(car);
                }

                db.SaveChanges();
            }
        }

        private static void QueryXml()
        {
            var ns = (XNamespace)"http:/pluralsight.com/cars/2016";
            var ex = (XNamespace)"http:/pluralsight.com/cars/2016/ex";

            var document = XDocument.Load("fuel.xml");

            var query =
                 from element in document.Element(ns + "Cars")?.Elements(ex + "Car")
                                                        ?? Enumerable.Empty<XElement>()
                 where element.Attribute("Manufacturer")?.Value == "BMW"
                 select element.Attribute("Name")?.Value;

            foreach (var name in query)
            {
                Console.WriteLine(name);
            }
        }

        private static void CreateXml()
        {
            //var records = ProcessCars("fuel.csv");

            //var document = new XDocument();
            //var cars = new XElement("Cars",
            //    from record in records
            //    select new XElement("Car",
            //                new XAttribute("Name", record.Name),
            //                new XAttribute("Combined", record.Combined),
            //                new XAttribute("Manufacturer", record.Manufacturer)));

            //document.Add(cars);
            //document.Save("fuel.xml");

            //Working with XML Namespaces
            var records = ProcessCars("fuel.csv");

            var ns = (XNamespace)"http:/pluralsight.com/cars/2016";
            var ex = (XNamespace)"http:/pluralsight.com/cars/2016/ex";

            var document = new XDocument();
            var cars = new XElement(ns + "Cars",
                from record in records
                select new XElement(ex + "Car",
                            new XAttribute("Name", record.Name),
                            new XAttribute("Combined", record.Combined),
                            new XAttribute("Manufacturer", record.Manufacturer)));

            cars.Add(new XAttribute(XNamespace.Xmlns + "ex", ex));

            document.Add(cars);
            document.Save("fuel.xml");
        }

        private static List<Car> ProcessCars(string path)
        {
            var query =
                File.ReadAllLines(path)
                .Skip(1)
                .Where(l => l.Length > 1)
                .Select(Car.ParseFromCsv)
                .ToList();

            return query;
        }
    }
}