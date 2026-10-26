using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.Models;

namespace StudentAPI.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly StudentDbContext db;

        public StudentsController(StudentDbContext context)
        {
            db = context;
        }

        // STANDARD RESPONSE
        private IActionResult Success<T>(T data)
        {
            object? responseData = data switch
            {
                StudentRow student => ToStudentResponse(student),
                IEnumerable<StudentRow> students => students.Select(ToStudentResponse).ToList(),
                _ => data
            };

            return Ok(new
            {
                success = true,
                data = responseData,
                error = (string?)null
            });
        }

        private static object ToStudentResponse(StudentRow student)
        {
            return new
            {
                id = student.Id,
                lastname = student.Lastname,
                firstname = student.Firstname,
                middlename = student.Middlename,

                acadlevel_id = student.Acadlevel_Id ?? 1,
                acadlevel_name = student.Acadlevel_Name ?? "No Levels",

                section_id = student.Section_Id ?? 1,
                section_name = student.Section_Name ?? "No Section",

                gender = student.Gender,
                email = student.Email
            };
        }

        private IActionResult Error(
            string message,
            int statusCode = 400)
        {
            return StatusCode(statusCode, new
            {
                success = false,
                data = (object?)null,
                error = message
            });
        }

        private async Task<string?> ValidateStudentRelations(
            int? acadlevelId,
            int? sectionId)
        {
            if (!acadlevelId.HasValue)
                return "Academic level ID is required.";

            if (!sectionId.HasValue)
                return "Section ID is required.";

            if (!await db.AcadLevels.AnyAsync(a => a.Id == acadlevelId.Value))
                return "Academic level ID does not exist.";

            if (!await db.Sections.AnyAsync(s => s.Id == sectionId.Value))
                return "Section ID does not exist.";

            return null;
        }

        // STUDENT QUERY WITH JOINED DATA
        private IQueryable<StudentRow> StudentRows()
        {
            return
                from s in db.Students.AsNoTracking()

                join a in db.AcadLevels
                    on s.AcadlevelId equals a.Id into levels
                from a in levels.DefaultIfEmpty()

                join t in db.Sections
                    on s.SectionId equals t.Id into sections
                from t in sections.DefaultIfEmpty()

                select new StudentRow
                {
                    Id = s.Id,
                    Lastname = s.Lastname,
                    Firstname = s.Firstname,
                    Middlename = s.Middlename,

                    Acadlevel_Id = s.AcadlevelId,
                    Acadlevel_Name = a == null ? null : a.Name,

                    Section_Id = s.SectionId,
                    Section_Name = t == null ? null : t.Name,

                    Gender = s.Gender,
                    Email = s.Email
                };
        }

        // GET: STUDENTS, SEARCH, DROPDOWN DATA
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] int? id,
            [FromQuery] string? search)
        {
            // GET ALL STUDENTS
            // ?get-all-students
            if (Request.Query.ContainsKey("get-all-students"))
            {
                var students = await StudentRows()
                    .OrderBy(s => s.Id)
                    .ToListAsync();

                return Success(students);
            }

            // GET ALL ACADEMIC LEVELS
            // ?get-all-acadlevel
            if (Request.Query.ContainsKey("get-all-acadlevel"))
            {
                var levels = await db.AcadLevels
                    .AsNoTracking()
                    .OrderBy(x => x.Id)
                    .ToListAsync();

                return Success(levels);
            }

            // GET ALL SECTIONS
            // ?get-all-sections
            if (Request.Query.ContainsKey("get-all-sections"))
            {
                var sections = await db.Sections
                    .AsNoTracking()
                    .OrderBy(x => x.Id)
                    .ToListAsync();

                return Success(sections);
            }

            // GET STUDENT BY ID
            // ?id=1
            if (id.HasValue)
            {
                var student = await StudentRows()
                    .FirstOrDefaultAsync(s => s.Id == id.Value);

                if (student == null)
                    return Error("Student not found.", 404);

                return Success(student);
            }

            // SEARCH STUDENTS
            // ?search=John
            if (Request.Query.ContainsKey("search"))
            {
                if (string.IsNullOrWhiteSpace(search))
                {
                    return Success(await StudentRows()
                        .ToListAsync());
                }

                string term = $"%{search.Trim()}%";

                var ids = db.Students
                    .Where(s =>
                        EF.Functions.ILike(s.Firstname, term) ||
                        EF.Functions.ILike(s.Lastname, term) ||
                        EF.Functions.ILike(s.Email, term))
                    .Select(s => s.Id);

                var students = await StudentRows()
                    .Where(s => ids.Contains(s.Id))
                    .ToListAsync();

                return Success(students);
            }

            return Error("Invalid GET request.");
        }

        // POST: ADD STUDENT
        [HttpPost]
        public async Task<IActionResult> Add(
            [FromBody] Student student)
        {
            var validationError = await ValidateStudentRelations(
                student.AcadlevelId, student.SectionId);

            if (validationError != null)
                return Error(validationError);

            student.Id = 0;

            db.Students.Add(student);
            await db.SaveChangesAsync();

            // Return newly inserted student
            var createdStudent = await StudentRows()
                .FirstOrDefaultAsync(s => s.Id == student.Id);

            return Success(createdStudent);
        }

        // PUT: UPDATE STUDENT
        [HttpPut]
        public async Task<IActionResult> Update(
            [FromBody] Student input)
        {
            if (input.Id <= 0)
                return Error("Invalid student ID.");

            var student = await db.Students
                .FindAsync(input.Id);

            if (student == null)
                return Error("Student not found.", 404);

            // Preserve existing links when the request omits their IDs.
            var acadlevelId = input.AcadlevelId ?? student.AcadlevelId;
            var sectionId = input.SectionId ?? student.SectionId;

            var validationError = await ValidateStudentRelations(
                acadlevelId, sectionId);

            if (validationError != null)
                return Error(validationError);

            student.Lastname = input.Lastname;
            student.Firstname = input.Firstname;
            student.Middlename = input.Middlename;
            student.AcadlevelId = acadlevelId;
            student.SectionId = sectionId;
            student.Gender = input.Gender;
            student.Email = input.Email;

            await db.SaveChangesAsync();

            var updatedStudent = await StudentRows()
                .FirstOrDefaultAsync(s => s.Id == input.Id);

            return Success(updatedStudent);
        }

        // DELETE: DELETE STUDENT
        [HttpDelete]
        public async Task<IActionResult> Delete(
            [FromBody] DeleteStudentRequest input)
        {
            if (input.Id <= 0)
                return Error("Invalid student ID.");

            var student = await db.Students
                .FindAsync(input.Id);

            if (student == null)
                return Error("Student not found.", 404);

            db.Students.Remove(student);
            await db.SaveChangesAsync();

            return Success(new
            {
                id = input.Id,
                message = "Student deleted successfully."
            });
        }
    }

    // DELETE REQUEST MODEL
    public class DeleteStudentRequest
    {
        public int Id { get; set; }
    }
}