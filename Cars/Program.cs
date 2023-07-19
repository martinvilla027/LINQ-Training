using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;

namespace Cars
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var cars = ProcessFile("fuel.csv");
            //UNIT 6
            var manufacturers = ProcessManufacturers("manufacturers.csv");

            // Method syntax
            var query =
                cars.OrderByDescending(c => c.Combined)
                .ThenBy(c => c.Name); // second sort

            // Query syntax
            var query2 = from car in cars
                         orderby car.Combined descending, car.Name ascending
                         select car;


            // UNIT 5: Filtering with Where and First
            var query3 = from car in cars
                         where car.Manufacturer == "BMW" && car.Year == 2016
                         orderby car.Combined descending, car.Name ascending
                         select car;

            var top =
                cars
                .OrderByDescending(c => c.Combined)
                .ThenBy(c => c.Name) // second sort
                .Select(c => c)
                .FirstOrDefault(c => c.Manufacturer == "BMW" && c.Year == 2016); //return a single car

            // UNIT 5 Quantifying Data with Any, All, Contains (all these operators return true or false)

            // Any: is there anything in this data set?
            var result = cars.Any(c => c.Manufacturer == "Ford");
            // true

            // All: if All dataset elements match with the condition(predicate)
            var result2 = cars.All(c => c.Manufacturer == "Ford");
            // false


            //UNIT 5 Projecting data with Select
            var a = from car in cars
                    where car.Manufacturer == "BMW" && car.Year == 2016
                    orderby car.Combined descending, car.Name ascending
                    select car;

            //UNIT 5 Flattening data with select many
            var result3 = cars.SelectMany(c => c.Name)
                              .OrderBy(c => c);

            //UNIT 6 Joining data with Query sintax
            var join =
                from car in cars
                join manufacturer in manufacturers on car.Manufacturer equals manufacturer.Name
                orderby car.Combined descending, car.Name ascending
                select new
                {
                    manufacturer.Headquarters,
                    car.Name,
                    car.Combined
                };

            //UNIT 6 Joining data with Method sintax

            var join2 =
                cars.Join(manufacturers,
                            c => c.Manufacturer,
                            m => m.Name, (c, m) => new
                            {
                                m.Headquarters,
                                c.Name,
                                c.Combined
                            })
                        .OrderByDescending(c => c.Combined)
                        .ThenBy(c => c.Name);

            //cars.Join(manufacturers,
            //            c => c.Manufacturer,
            //            m => m.Name, (c, m) => new
            //            {
            //                Car = c,
            //                Manufacturer = m
            //            })
            //        .OrderByDescending(c => c.Car.Combined)
            //        .ThenBy(c => c.Car.Name)
            //        .Select(c => new
            //        {
            //            c.Manufacturer.Headquarters,
            //            c.Car.Name,
            //            c.Car.Combined
            //        });

            //UNIT 6 Join with a composite key, query sintax
            var join3 =
                from car in cars
                join manufacturer in manufacturers
                on new { car.Manufacturer, car.Year }
                equals
                new { Manufacturer = manufacturer.Name, manufacturer.Year }
                orderby car.Combined descending, car.Name ascending
                select new
                {
                    manufacturer.Headquarters,
                    car.Name,
                    car.Combined
                };

            //UNIT 6 Join with a composite key, method sintax
            var join4 =
                cars.Join(manufacturers,
                            c => new { c.Manufacturer, c.Year },
                            m => new { Manufacturer = m.Name, m.Year },
                            (c, m) => new
                            {
                                m.Headquarters,
                                c.Name,
                                c.Combined
                            })
                        .OrderByDescending(c => c.Combined)
                        .ThenBy(c => c.Name);

            //foreach (var car in join4.Take(10))
            //{
            //    Console.WriteLine($"{car.Headquarters} {car.Name} : {car.Combined}");
            //}

            //UNIT 6 Grouping data, query sintax
            var grouping =
                from car in cars
                group car by car.Manufacturer.ToUpper() into manufacturer
                orderby manufacturer.Key
                select manufacturer;

            //UNIT 6 Grouping data, method sintax
            var grouping2 =
                cars.GroupBy(c => c.Manufacturer.ToUpper())
                    .OrderBy(g => g.Key);

            //foreach (var group in grouping2)
            //{
            //    Console.WriteLine(group.Key);
            //    foreach (var car in group.OrderByDescending(c => c.Combined).Take(2))
            //    {
            //        Console.WriteLine($"\t{car.Name}: {car.Combined}");
            //    }
            //}

            //UNIT 6 GroupJoin for hierarchicaldata, query sintax
            var groupjoin =
                from manufacturer in manufacturers
                join car in cars on manufacturer.Name equals car.Manufacturer
                    into carGroup
                select new
                {
                    Manufacturer = manufacturer,
                    Cars = carGroup
                };

            //UNIT 6 GroupJoin for hierarchicaldata, method sintax
            var groupjoin2 =
                manufacturers.GroupJoin(cars, m => m.Name, c => c.Manufacturer,
                (m, g) =>
                new
                {
                    Manufacturer = m,
                    Cars = g
                }).OrderBy(m => m.Manufacturer.Name);

            //foreach (var group in groupjoin2)
            //{
            //    Console.WriteLine($"{group.Manufacturer.Name} : {group.Manufacturer.Headquarters}");
            //    foreach (var car in group.Cars.OrderByDescending(c => c.Combined).Take(2))
            //    {
            //        Console.WriteLine($"\t{car.Name}: {car.Combined}");
            //    }
            //}

            //UNIT 6 groupby country challenge, query sintax
            var country =
                from manufacturer in manufacturers
                join car in cars on manufacturer.Name equals car.Manufacturer
                    into carGroup
                select new
                {
                    Manufacturer = manufacturer,
                    Cars = carGroup
                } into challenge
                group challenge by challenge.Manufacturer.Headquarters;

            //UNIT 6 groupby country challenge, method sintax
            var country2 =
                manufacturers.GroupJoin(cars, m => m.Name, c => c.Manufacturer,
                (m, g) =>
                new
                {
                    Manufacturer = m,
                    Cars = g
                }).GroupBy(m => m.Manufacturer.Headquarters);

            //foreach (var group in country2)
            //{
            //    Console.WriteLine($"{group.Key}");
            //    foreach (var car in group.SelectMany(g => g.Cars)
            //                             .OrderByDescending(c => c.Combined)
            //                             .Take(3))
            //    {
            //        Console.WriteLine($"\t{car.Name}: {car.Combined}");
            //    }
            //}

            //Aggregating data, query syntax
            var aggregating =
                from car in cars
                group car by car.Manufacturer into carGroup
                select new
                {
                    Name = carGroup.Key,
                    Max = carGroup.Max(c => c.Combined),
                    Min = carGroup.Min(c => c.Combined),
                    Average = carGroup.Average(c => c.Combined)
                } into res
                orderby res.Max descending
                select res;

            //Efficient aggregation, method syntax
            var efficient =
                cars.GroupBy(c => c.Manufacturer)
                .Select(g =>
                {
                    var results = g.Aggregate(new CarStatistics(),
                        (acc, c) => acc.Accumulate(c),
                        acc => acc.Compute());
                    return new
                    {
                        Name = g.Key,
                        Avg = results.Average,
                        Min = results.Min,
                        Max = results.Max
                    };
                })
                .OrderByDescending(r => r.Max);

            foreach (var agg in efficient)
            {
                Console.WriteLine($"{agg.Name}");
                Console.WriteLine($"\t Max: {agg.Max}");
                Console.WriteLine($"\t Min: {agg.Min}");
                Console.WriteLine($"\t Avg: {agg.Avg}");
            }
        }

        private static List<Manufacturer> ProcessManufacturers(string path)
        {
            //Method syntax
            var query =
                File.ReadAllLines(path)
                    .Where(l => l.Length > 1)
                    .Select(l =>
                    {
                        var columns = l.Split(',');
                        return new Manufacturer
                        {
                            Name = columns[0],
                            Headquarters = columns[1],
                            Year = int.Parse(columns[2])
                        };
                    });

            return query.ToList();
        }

        private static List<Car> ProcessFile(string path)
        {
            //Method syntax
            return
                File.ReadAllLines(path)
                    .Skip(1) // Skip the first line of the file
                    .Where(line => line.Length > 1) // Checking if there is car information in a line (remove last line)
                    .Select(Car.ParseFromCsv)
                    .ToList();

            //Query sintax
            //var query =
            //    from line in File.ReadAllLines(path).Skip(1)
            //    where line.Length > 1
            //    select Car.ParseFromCsv(line);

            //return query.ToList();

        }

        public class CarStatistics
        {
            public CarStatistics()
            {
                Max = Int32.MinValue;
                Min = Int32.MaxValue;
            }

            public CarStatistics Accumulate(Car car)
            {
                Count += 1;
                Total += car.Combined;
                Max = Math.Max(Max, car.Combined);
                Min = Math.Min(Min, car.Combined);
                return this;
            }

            public CarStatistics Compute()
            {
                Average = Total / Count;
                return this;
            }

            public int Max { get; set; }
            public int Min { get; set; }
            public int Total { get; set; }
            public int Count { get; set; }
            public double Average { get; set; }
        }
    }
}