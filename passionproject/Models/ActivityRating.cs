using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace passionproject.Itinerary
{
    public class ActivityRating
    {
        [Key]
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public int MemberId { get; set; }
        public int Rating { get; set; }  // 1-5 stars
        public string Comment { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public Activity Activity { get; set; }
        
        [System.Text.Json.Serialization.JsonIgnore]
        public Member Member { get; set; }
    }
}