using System.ComponentModel.DataAnnotations.Schema;

namespace FlightSchedule.Models
{
    [Table("subscriptions")]
    public class Subscription
    {
        [Column("origin_city_id")]
        public int OriginCityId { get; set; }

        [Column("destination_city_id")]
        public int DestinationCityId { get; set; }

        [Column("agency_id")]
        public int AgencyId { get; set; } 
    }
}
