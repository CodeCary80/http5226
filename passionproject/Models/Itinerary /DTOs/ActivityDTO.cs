namespace passionproject.Models.Itinerary.DTOs
{
    public class ActivityDTO
    {
        public int ActivityId { get; set; }
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
        public int DestinationId { get; set; }
        public DestinationSummaryDTO Destination { get; set; }
        public List<MemberSummaryDTO> Members { get; set; }
    }

    public class DestinationSummaryDTO
    {
        public int DestinationId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
    }
}