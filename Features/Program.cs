namespace Features
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Func<int, int> square = x => x * x;
            Func<int, int, int> add = (x, y) =>  x + y;

            Console.WriteLine(square(add(3, 5)));
            Console.WriteLine("\n");

            Action<int> write = x => Console.WriteLine(x);
            write(square(add(3, 5)));
            Console.WriteLine("\n");

            Employee[] developers = new Employee[]
            {
                new Employee { Id = 1, Name = "Scoot" },
                new Employee { Id = 2, Name = "Chris" }
            };

            List<Employee> sales = new List<Employee>()
            {
                new Employee { Id = 3, Name = "Alex" }
            };

            // Array and List has GetEnumerator method, because they implement the IEnumerable interface
            // We can use foreach for both

            IEnumerable<Employee> developers2 = new Employee[]
            {
                new Employee { Id = 1, Name = "Scoot" },
                new Employee { Id = 2, Name = "Chris" }
            };

            IEnumerable<Employee> sales2 = new List<Employee>()
            {
                new Employee { Id = 3, Name = "Alex" }
            };

            // Extension method created in MyLinq
            Console.WriteLine(sales2.Count());

            Console.WriteLine("\n");

            IEnumerator<Employee> enumerator = developers2.GetEnumerator();
            while(enumerator.MoveNext())
            {
                Console.WriteLine(enumerator.Current.Name);  
            }

            Console.WriteLine("\n");
            //IEnumerable is the perfect interface for hiding the source data


            // Ordering cases
            foreach (var employee in developers2.Where(NameStartWithS))
            {
                Console.WriteLine(employee.Name);
            }

            Console.WriteLine("\n");

            // Anonimous method ordering case
            foreach (var employee in developers2.Where(
                delegate(Employee employee)
                {
                     return employee.Name.StartsWith("S");
                }))
            {
                Console.WriteLine(employee.Name);
            }

            Console.WriteLine("\n");

            // Lambda expresion case
            foreach (var employee in developers2.Where(
                e => e.Name.StartsWith("S")))
            {
                Console.WriteLine(employee.Name);
            }

            Console.WriteLine("\n");

            // Ascending order name with length condition
            foreach (var employee in developers2.Where(e => e.Name.Length == 5)
                                                .OrderBy(e => e.Name))
            {
                Console.WriteLine(employee.Name);
            }

            Console.WriteLine("\n");

            // Method syntax
            var query = developers2.Where(e => e.Name.Length == 5)
                                    .OrderBy(e => e.Name);

            // Query syntax
            var query2 = from developer in developers2
                         where developer.Name.Length == 5
                         orderby developer.Name
                         select developer;

            foreach (var employee in query2)
            {
                Console.WriteLine(employee.Name);
            }

        }

        private static bool NameStartWithS(Employee employee)
        {
            return employee.Name.StartsWith("S");
        }
    }
}