namespace passionproject.Models.Itinerary.DTOs
{
    public class ExpenseSplitDTO
    {
      public int MemberId { get; set; }
      public string MemberName { get; set; }
      public decimal Share { get; set; }
      public bool IsPaid { get; set; }
    }
}