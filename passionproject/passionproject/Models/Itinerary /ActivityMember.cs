using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace passionproject.Itinerary
{
    public class ActivityMember
    {
        public int ActivityId { get; set; }
        public int MemberId { get; set; }
        
        public bool IsOrganizer { get; set; }
        public string? Notes { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public Activity? Activity { get; set; }
        
        [System.Text.Json.Serialization.JsonIgnore]
        public Member? Member { get; set; }
    }
}