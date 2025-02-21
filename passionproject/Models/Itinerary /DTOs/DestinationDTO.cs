namespace passionproject.Models.Itinerary.DTOs
{
    public class DestinationDTO
    {
        public int DestinationId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public decimal Budget { get; set; }
        public List<ActivityDTO> Activities { get; set; }
    }
}