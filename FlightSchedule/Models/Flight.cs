using System.ComponentModel.DataAnnotations.Schema;

namespace FlightSchedule.Models
{
    [Table("flights")]
    public class Flight
    {
        [Column("flight_id")]
        public int FlightId { get; set; }

        [Column("route_id")]
        public int RouteId { get; set; }

        [Column("departure_time")]
        public DateTime DepartureTime { get; set; }

        [Column("arrival_time")]
        public DateTime ArrivalTime { get; set; }

        [Column("airline_id")]
        public int AirlineId { get; set; }

        public Route Route { get; set; }

    }

    public enum FlightStatus
    {
        USUAL = 0,
        NEW = 1,
        DISCONTINUED = 2

    }

    public class FlightResult
    {
        public int FlightId { get; set; }
        public int OriginCityId { get; set; }
        public int DestinationCityId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int AirlineId { get; set; }
        public FlightStatus Status { get; set; }

    }
}
