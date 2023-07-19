namespace Queries
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var movies = new List<Movie>
            {
                new Movie { Title = "The Dark Knight", Rating = 8.9f, Year = 2008 },
                new Movie { Title = "The King's Speech", Rating = 8.0f, Year = 2010 },
                new Movie { Title = "Casablanca", Rating = 8.5f, Year = 1942 },
                new Movie { Title = "Star Wars V", Rating = 8.7f, Year = 1980 }
            };


            var query = movies.Filter(m => m.Year > 2000);

            foreach (var movie in query)
            {
                Console.WriteLine(movie.Title);
            }

            Console.WriteLine("\n");

            //Deferred execution advantage
 
            query = query.Take(1);
            var enumerator = query.GetEnumerator();

            while (enumerator.MoveNext())
            {
                Console.WriteLine(enumerator.Current.Title);
            }

            Console.WriteLine("\n");

            // methods that return some abstract type like IEnumerable,
            // they will offer deferred execution

            // Operators like ToList don't return an abstract type, this returns a concrete list of T
            // It doesn't implement deferred execution, it produces a real list with items inside of it
            var query2 = movies.Where(m => m.Year > 2000).ToList();

            Console.WriteLine(query2.Count);
            var enumerator2 = query2.GetEnumerator();

            while (enumerator2.MoveNext())
            {
                Console.WriteLine(enumerator2.Current.Title);
            }

        }
    }
}