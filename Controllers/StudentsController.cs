using Microsoft.AspNetCore.Mvc;

namespace StudentAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetStudents()
        {
            var students = new[]
            {
                new { Id = 1, Name = "Juan Dela Cruz" },
                new { Id = 2, Name = "Maria Santos" },
                new { Id = 3, Name = "Pedro Reyes" }
            };

            return Ok(students);
        }

        [HttpGet("{id}")]
        public IActionResult GetStudentById(int id)
        {
            return Ok(new
            {
                Id = id,
                Name = "Sample Student"
            });
        }
    }
}