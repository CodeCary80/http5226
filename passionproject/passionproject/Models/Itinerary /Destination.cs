using System.ComponentModel.DataAnnotations;

namespace passionproject.Itinerary
{
    public class Destination
    {
        public Destination()
        {
            Activities = new List<Activity>();
        }

        [Key]
        public int DestinationId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public decimal Budget { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<Activity> Activities { get; set; }
    }
}