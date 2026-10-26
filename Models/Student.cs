using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentApi.Models;


[Table("students")]
public class Student
{
    [Key, Column("id")]
    public int Id { get; set; }
    [Required, MaxLength(100), Column("lastname")]
    public string Lastname { get; set; } = "";
    [Required, MaxLength(100), Column("firstname")]
    public string Firstname { get; set; } = "";
    [MaxLength(100), Column("middlename")]
    public string? Middlename { get; set; }
    [Column("acadlevel_id")]
    public int? AcadlevelId { get; set; }
    [Column("section_id")]
    public int? SectionId { get; set; }
    [Required, StringLength(1), Column("gender")]
    public string Gender { get; set; } = "";
    [Required, MaxLength(100), Column("email")]
    public string Email { get; set; } = "";
}