using System.ComponentModel.DataAnnotations;

namespace passionproject.Models.ViewModels
{
    public class ActivityViewModel
    {
        [Required]
        public int ActivityId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime DateTime { get; set; }
        [Required]
        public string Location { get; set; }
        public string Description { get; set; }
        [Required]
        public decimal Cost { get; set; }
        public List<MemberViewModel> Members { get; set; } = new();
    }
}