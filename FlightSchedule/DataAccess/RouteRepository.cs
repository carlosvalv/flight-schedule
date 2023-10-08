
using FlightSchedule.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightSchedule.DataAccess
{
    public class RouteRepository
    {
        private readonly AppDbContext _dbContext;

        public RouteRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Route> Get(int agencyId, DateTime init, DateTime end)
        {
            try
            {
                var routes = _dbContext.Routes
                    .Include(route => route.Flights)
                    .Join(_dbContext.Subscriptions,
                          r => new { r.OriginCityId, r.DestinationCityId },
                          s => new { s.OriginCityId, s.DestinationCityId },
                          (r, sub) => new { Route = r, Subscription = sub })
                    .Where(joined => joined.Subscription.AgencyId == agencyId)
                    .Where(joined => joined.Route.DepartureDate > init.AddDays(-7) && joined.Route.DepartureDate < end.AddDays(7))
                    .Select(joined => joined.Route)
                    .Distinct()
                    .ToList();

                return routes;
            }
            catch (Exception ex)
            {
                // Maneja la excepción de manera adecuada (registra, notifica, etc.).
                throw;
            }
        }

    }
}
