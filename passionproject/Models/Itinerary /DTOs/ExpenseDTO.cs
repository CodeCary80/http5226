namespace passionproject.Models.Itinerary.DTOs
{
    public class ExpenseDTO
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public List<ExpenseSplitDTO> Splits { get; set; }
    }
}