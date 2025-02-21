using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace passionproject.Itinerary
{   
    public class Expense
    {
        public Expense()
       {
        ExpenseSplits = new List<ExpenseSplit>();
       }
        [Key]
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public Activity Activity { get; set; }
    
        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<ExpenseSplit> ExpenseSplits { get; set; }
    }

        public class ExpenseSplit
{
        [Key]
        public int Id { get; set; }
        public int ExpenseId { get; set; }
        public int MemberId { get; set; }
        public decimal Share { get; set; }
        public bool IsPaid { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public Expense Expense { get; set; }
    
        [System.Text.Json.Serialization.JsonIgnore]
        public Member Member { get; set; }
}
}