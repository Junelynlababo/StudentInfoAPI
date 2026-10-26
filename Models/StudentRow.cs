namespace StudentApi.Models;
public class StudentRow
{
    public int Id { get; set; }
    public string Lastname { get; set; } = "";
    public string Firstname { get; set; } = "";
    public string? Middlename { get; set; }
    public int? Acadlevel_Id { get; set; }
    public string? Acadlevel_Name { get; set; }
    public int? Section_Id { get; set; }
    public string? Section_Name { get; set; }
    public string Gender { get; set; } = "";
    public string Email { get; set; } = "";
}