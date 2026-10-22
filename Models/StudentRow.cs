namespace StudentAPI.Models;

public class StudentRow
{
    public int Id { get; set; }
    public string Lastname { get; set; } = "";
    public string Firstname { get; set; } = "";
    public string? Middlename { get; set; }
    public int? AcadlevelId { get; set; }
    public string? AcadlevelName { get; set; }
    public int? SectionId { get; set; }
    public string? SectionName { get; set; }
    public string Gender { get; set; } = "";
    public string Email { get; set; } = "";
}