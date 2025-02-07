namespace passionproject.Models.Itinerary.DTOs
{
    public class MemberDTO
    {
        public int MemberId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string DietaryRestrictions { get; set; }
        public string HealthConsiderations { get; set; }
        public string EmergencyContact { get; set; }
        public List<ActivityDetailDTO> Activities { get; set; }
    }

    public class MemberSummaryDTO
    {
        public int MemberId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsOrganizer { get; set; }
        public string Notes { get; set; }
    }

    public class ActivityDetailDTO
    {
        public int ActivityId { get; set; }
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
        public bool IsOrganizer { get; set; }
        public string Notes { get; set; }
        public DestinationSummaryDTO Destination { get; set; }
    }
}