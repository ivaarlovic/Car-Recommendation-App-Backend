using CarRecommendationApp.Data;
using CarRecommendationApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarRecommendationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyUserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SurveyUserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetSurveyUsers()
        {
            var surveyUsers = _context.SurveyUsers.ToList();
            return Ok(surveyUsers);
        }

        [HttpPost]
        public IActionResult AddSurveyUsers([FromBody] SurveyUser surveyUser)
        {
            var existingUser =
                _context.SurveyUsers
                .FirstOrDefault(x =>
                x.Email == surveyUser.Email);

            if (existingUser != null) {
                return Ok(existingUser);
            }

            _context.SurveyUsers.Add(surveyUser);
            _context.SaveChanges();

            return Ok(surveyUser);
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteSurvey(int id)
        {
            var user = await _context.SurveyUsers.FindAsync(id);
            if (user == null) 
                return NotFound();

            user.IsCompleted = true;
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
