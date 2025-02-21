namespace passionproject.Models.ViewModels
{
    public class ExpenseViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public List<ExpenseSplitViewModel> Splits { get; set; } = new();
    }

    public class ExpenseSplitViewModel
    {
        public string MemberName { get; set; }
        public decimal Share { get; set; }
        public bool IsPaid { get; set; }
    }
}