namespace passionproject.Models.ViewModels
{
    public class MemberViewModel
{
    public int MemberId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string DietaryRestrictions { get; set; }
    public string HealthConsiderations { get; set; }
    public string EmergencyContact { get; set; }
    public bool IsOrganizer { get; set; }
    public string Notes { get; set; }
}
}