using System;
using System.Linq;
using System.Collections.Generic;


namespace WeatherLab
{
    class Program
    {
        static string dbfile = @".\data\climate.db";

        static void Main(string[] args)
        {
            var measurements = new WeatherSqliteContext(dbfile).Weather;

            var total_2020_precipitation = measurements.Where(mt=>mt.year == 2020)
                                           .Sum(mt=>mt.precipitation);
            Console.WriteLine($"Total precipitation in 2020: {total_2020_precipitation} mm\n");
         
            var hdd = measurements
                
                .GroupBy(mt => mt.year)

               .Select(
                   groupByYear => new {
                       year = groupByYear.Key,
                       Hd = groupByYear.Where(row => row.meantemp < 18).Count()

                   }
                );
            var cdd = measurements
                .Where(mt => mt.meantemp >= 18)
                .GroupBy(mt => mt.year)
                   .Select(
                   groupByYear => new {
                       year = groupByYear.Key,
                       cd = groupByYear.Where(row => row.meantemp >= 18).Count()

                   }
                );


          
            var grouping = from h in hdd
                        join c in cdd on h.year equals c.year
                         orderby h.year
                         select new { Year = h.year, hdd = h.Hd, cdd = c.cd };
          



            Console.WriteLine("Year\thdd\tcdd");
            foreach (var i in grouping)
            {
                Console.WriteLine($"{i.Year}\t{i.hdd}\t{i.cdd}");
            }
           
               


            Console.WriteLine("\nTop 5 Most Variable Days");
            Console.WriteLine("YYYY-MM-DD\tDelta");
            var mvd = measurements.Select(mt => new
            {
                year = mt.year,
                month = mt.month,
                day = mt.day,
                delta = mt.maxtemp - mt.mintemp,
            });
            var top5 = mvd
                                                .OrderByDescending(measurement => measurement.delta)
                                                .Take(5);
            foreach (var day in top5)
            {
                Console.WriteLine($"{day.year:d4}-{day.month:d2}-{day.day:d2}\t{day.delta:0.00}");
            }
        }
    }
}
