using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace StudentAPI.Models;


[Table("sections")]
public class Section
{
    [Key, Column("id")] public int Id { get; set; }
    [Required, MaxLength(100), Column("name")] public string Name { get; set; } = "";
}
