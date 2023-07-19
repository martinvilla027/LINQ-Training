using Cars;
using System.Xml.Linq;

namespace XMLModule
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CreateXml();
            QueryXml();
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