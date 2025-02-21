namespace passionproject.Models.Itinerary.DTOs
{
    public class ActivityMemberDTO
    {
        public int ActivityId { get; set; }
        public int MemberId { get; set; }
        public bool IsOrganizer { get; set; }
        public string Notes { get; set; }
        public ActivityDTO Activity { get; set; }
        public MemberDTO Member { get; set; }
    }
}