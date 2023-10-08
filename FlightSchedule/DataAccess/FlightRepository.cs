using FlightSchedule.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightSchedule.DataAccess
{
    public class FlightRepository
    {
        private readonly AppDbContext _dbContext;

        public FlightRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Flight? GetFlightById(int flightId)
        {
            try
            {
                return _dbContext.Flights
                    .Include(f => f.Route)
                    .FirstOrDefault(f => f.FlightId == flightId);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<Flight> GetFlightsByAirlineId(int airlineId)
        {
            try
            {
                return _dbContext.Flights
                    .Include(f => f.Route)
                    .Where(f => f.AirlineId == airlineId).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
