using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAPI.Data;
using StudentAPI.Models;

namespace StudentAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController(StudentDbContext db) : ControllerBase
    {
      
       private IQueryable<StudentRow> StudentRows() =>
        from s in db.Students.AsNoTracking()
        join a in db.AcadLevels on s.AcadlevelId equals a.Id into levels
        from a in levels.DefaultIfEmpty()
        join t in db.Sections on s.SectionId equals t.Id into sections
        from t in sections.DefaultIfEmpty()
        select new StudentRow
        {
            Id = s.Id, Lastname = s.Lastname, Firstname = s.Firstname, Middlename = s.Middlename,
            AcadlevelId = s.AcadlevelId, AcadlevelName = a == null ? null : a.Name,
            SectionId = s.SectionId, SectionName = t == null ? null : t.Name,
            Gender = s.Gender, Email = s.Email
        };

     [HttpGet]
        public async Task<ActionResult<List<StudentRow>>> GetStudents()
        {
            var students = await StudentRows().ToListAsync();
            return Ok(students);
        }

    }
}