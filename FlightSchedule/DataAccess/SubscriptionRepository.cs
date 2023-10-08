using FlightSchedule.Models;

namespace FlightSchedule.DataAccess
{
    public class SubscriptionRepository
    {
        private readonly AppDbContext _dbContext;

        public SubscriptionRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Subscription> GetSubscriptionsByAgencyId(int agencyId)
        {
            try
            {
                return _dbContext.Subscriptions
                .Where(s => s.AgencyId == agencyId)
                .ToList();
            }
            catch (Exception ex)
            {
                // Maneja la excepción de manera adecuada (registra, notifica, etc.).
                throw;
            }
        }

    }
}
