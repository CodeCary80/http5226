using System.ComponentModel.DataAnnotations;

namespace passionproject.Itinerary
{
    public class Activity
    {
        public Activity()
        {
            ActivityMembers = new List<ActivityMember>();
        }

        [Key]
        public int ActivityId { get; set; }
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
        
        public int DestinationId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public Destination? Destination { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<ActivityMember> ActivityMembers { get; set; }
    }
}