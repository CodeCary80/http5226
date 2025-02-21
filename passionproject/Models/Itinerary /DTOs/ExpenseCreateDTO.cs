namespace passionproject.Models.Itinerary.DTOs
{
    public class ExpenseCreateDTO
    {
        public int ActivityId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public List<ExpenseSplitCreateDTO> Splits { get; set; }
    }
}