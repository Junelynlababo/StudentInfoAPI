using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace StudentAPI.Models;

[Table("acadlevels")]
public class AcadLevel
{
    [Key, Column("id")] public int Id { get; set; }
    [Required, MaxLength(100), Column("name")] public string Name { get; set; } = "";
}