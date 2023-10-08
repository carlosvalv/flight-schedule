using FlightSchedule.DataAccess;
using FlightSchedule;
using Microsoft.EntityFrameworkCore;
using FlightSchedule.Models;
using System.Text;
using System.Diagnostics;

namespace FlightScheduleDetection
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            var dbWatch = new Stopwatch();
            var algWatch = new Stopwatch();
            stopwatch.Start();
            dbWatch.Start();

            if (args.Length != 3)
            {
                Console.WriteLine("Usage: YourProgram.exe <start date> <end date> <agency id>");
                return;
            }

            var initDate = new DateTime(DateTime.Parse(args[0]).Ticks, DateTimeKind.Utc);
            var endDate = new DateTime(DateTime.Parse(args[1]).Ticks, DateTimeKind.Utc);
            var agencyId = int.Parse(args[2]);

            // Create an instance of your DbContext.
            using var dbContext = InitializeDbContext();
            var subscriptionRepository = new SubscriptionRepository(dbContext);
            var routeRepository = new RouteRepository(dbContext);

            var agencySubscriptions = subscriptionRepository.GetSubscriptionsByAgencyId(agencyId);
            var routes = routeRepository.Get(agencyId, initDate, endDate);

            dbWatch.Stop();
            algWatch.Start();

            var resultFlights = DetectFlights(routes, initDate, endDate);


            GenerateCSVFile(resultFlights, "results.csv");
            algWatch.Stop();
            stopwatch.Stop();
            TimeSpan elapsedTime = stopwatch.Elapsed;
            Console.WriteLine($"Execution time: {elapsedTime.TotalMilliseconds} ms");
            Console.WriteLine($"DB Time: {dbWatch.Elapsed.TotalMilliseconds} ms");
            Console.WriteLine($"Execution time: {algWatch.Elapsed.TotalMilliseconds} ms");

        }

        static AppDbContext InitializeDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql("Host=127.0.0.1;Port=5432;Database=flight_schedule;Username=postgres;Password=Carlos1;")
                .Options;

            return new AppDbContext(options);
        }

        static List<Flight> FindNewFlights(List<Flight> allFlights, List<Flight> filteredFlights)
        {
            return filteredFlights
                .Where(flight =>
                    !allFlights.Any(otherFlight =>
                        flight.FlightId != otherFlight.FlightId &&
                        IsFlightWithinDateTolerance(flight, otherFlight, -7, 30) &&
                        flight.AirlineId == otherFlight.AirlineId))
                .ToList();
        }

        static List<Flight> FindDiscontinuedFlights(List<Flight> allFlights, List<Flight> filteredFlights)
        {
            return filteredFlights
                .Where(flight =>
                    !allFlights.Any(otherFlight =>
                        flight.FlightId != otherFlight.FlightId &&
                        IsFlightWithinDateTolerance(flight, otherFlight, 7, 30) &&
                        flight.AirlineId == otherFlight.AirlineId))
                .ToList();
        }

        static bool IsFlightWithinDateTolerance(Flight flight1, Flight flight2, int daysTolerance, int minutesTolerance)
        {
            return flight1.DepartureTime >= flight2.DepartureTime.AddDays(daysTolerance).AddMinutes(-minutesTolerance) &&
                flight1.DepartureTime <= flight2.DepartureTime.AddDays(daysTolerance).AddMinutes(minutesTolerance);
        }

        static FlightResult CreateFlightResult(int originCityId, int destinationCityId, Flight flight, FlightStatus status)
        {
            return new FlightResult
            {
                OriginCityId = originCityId,
                DestinationCityId = destinationCityId,
                FlightId = flight.FlightId,
                AirlineId = flight.AirlineId,
                ArrivalTime = flight.ArrivalTime,
                DepartureTime = flight.DepartureTime,
                Status = status
            };
        }

        static List<FlightResult> DetectFlights(List<Route> routes, DateTime initDate, DateTime endDate)
        {
            var resultFlights = new List<FlightResult>();

            var groupedRoutes = routes
               .GroupBy(r => new { r.OriginCityId, r.DestinationCityId })
               .Select(group => new
               {
                   Routes = group.ToList(),
                   Flights = group.SelectMany(r => r.Flights).ToList()
               })
               .ToList();

            foreach (var route in groupedRoutes)
            {
                if(route.Flights.Count <= 1) continue;

                var flightsFilteredDate = route.Flights
                    .Where(flight =>
                        flight.DepartureTime >= initDate &&
                        flight.DepartureTime <= endDate
                    ).ToList();

                if (flightsFilteredDate.Count == 0) continue;

                var newFlights = FindNewFlights(route.Flights.ToList(), flightsFilteredDate);
                var discontinuedFlights = FindDiscontinuedFlights(route.Flights.ToList(), flightsFilteredDate);

                resultFlights.AddRange(newFlights.Select(flight => CreateFlightResult(route.Routes[0].OriginCityId, route.Routes[0].DestinationCityId, flight, FlightStatus.NEW)));
                resultFlights.AddRange(discontinuedFlights.Select(flight => CreateFlightResult(route.Routes[0].OriginCityId, route.Routes[0].DestinationCityId, flight, FlightStatus.DISCONTINUED)));
            }

            return resultFlights;
        }

        private static void GenerateCSVFile(List<FlightResult> flightResults, string filePath)
        {
            var csvContent = new StringBuilder();

            csvContent.AppendLine("flight_id,origin_city_id,destination_city_id,departure_time,arrival_time,airline_id,status");

            flightResults.ForEach(flight =>
            {
                csvContent.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6}", flight.FlightId, flight.OriginCityId, flight.DestinationCityId, flight.DepartureTime, flight.ArrivalTime, flight.AirlineId, flight.Status));
            });

            File.WriteAllText(filePath, csvContent.ToString());

            Console.WriteLine("CSV file created successfully.");
        }
    }
}