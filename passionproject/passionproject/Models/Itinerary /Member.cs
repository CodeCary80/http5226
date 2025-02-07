using System.ComponentModel.DataAnnotations;

namespace passionproject.Itinerary
{
    public class Member
    {
        public Member()
        {
            ActivityMembers = new List<ActivityMember>();
        }

        [Key]
        public int MemberId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string DietaryRestrictions { get; set; }
        public string HealthConsiderations { get; set; }
        public string EmergencyContact { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<ActivityMember> ActivityMembers { get; set; }
    }
}