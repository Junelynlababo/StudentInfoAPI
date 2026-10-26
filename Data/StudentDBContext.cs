using Microsoft.EntityFrameworkCore;
using StudentApi.Models;
namespace StudentApi.Data;
public class StudentDbContext(DbContextOptions<StudentDbContext> options) : DbContext(options)
{
    public DbSet<Student> Students => Set<Student>();
    public DbSet<AcadLevel> AcadLevels => Set<AcadLevel>();
    public DbSet<Section> Sections => Set<Section>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>().HasOne<AcadLevel>()
            .WithMany().HasForeignKey(s => s.AcadlevelId)
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Student>().HasOne<Section>()
            .WithMany().HasForeignKey(s => s.SectionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}