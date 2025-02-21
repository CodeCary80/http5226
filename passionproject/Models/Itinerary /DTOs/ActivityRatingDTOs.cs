namespace passionproject.Models.Itinerary.DTOs
{
    public class ActivityRatingDTO
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public int MemberId { get; set; }
        public string MemberName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }

    public class ActivityRatingCreateDTO
    {
        public int ActivityId { get; set; }
        public int MemberId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}