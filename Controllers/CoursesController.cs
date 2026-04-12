using LearningPlatform.Filters;
using LearningPlatform.Interfaces;
using LearningPlatform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LearningPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly IBackgroundTaskQueue _queue;

        public CoursesController(AppDbContext context,IMemoryCache cache,IBackgroundTaskQueue queue)
        {
            _context = context;
            _cache = cache;
            _queue = queue;
        }

        // GET: api/Courses
        [HttpGet("Get")]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            //return await _context.Courses.ToListAsync(); // في حالة مفيش cache
            return await _cache.GetOrCreateAsync("Courses", async e =>
            {
                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return await _context.Courses.ToListAsync();
            }


                );
            // *** another way to implement caching without using GetOrCreateAsync ***

            //const string cacheKey = "courses_list";

            //if (!_cache.TryGetValue(cacheKey, out List<Course> courses))
            //{
            //    courses = await _context.Courses.ToListAsync();

            //    var cacheOptions = new MemoryCacheEntryOptions()
            //        .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
            //        .SetSlidingExpiration(TimeSpan.FromMinutes(2));

            //    _cache.Set(cacheKey, courses, cacheOptions);
            //}

            //return Ok(courses);
        }

        // GET: api/Courses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            return course;
        }

        // PUT: api/Courses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse(int id, Course course)
        {
            if (id != course.Id)
            {
                return BadRequest();
            }

            _context.Entry(course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _cache.Remove("Courses");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Courses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PostCourse")]
        [ServiceFilter(typeof(InstructorOnlyFilter))]
        public async Task<ActionResult<Course>> PostCourse(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            _cache.Remove("Courses");

            return CreatedAtAction("GetCourse", new { id = course.Id }, course);
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            _cache.Remove("Courses");

            return NoContent();
        }
        [HttpPost("enroll")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId == null)
            { 
                return NotFound();
            }

            var enrollment = new Enrollment
            {
                CourseId = courseId,
                StudentId = int.Parse(userId),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Progress = 0,
            };
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            _queue.QueueTask(async token =>
            {
                await Task.Delay(3000); // simulate heavy work

                Console.WriteLine($"Enrollment processed for user {userId}");

                // simulate email/logging/etc.
            });

            return Ok("Enrollment submitted successfully");
        }
        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}
