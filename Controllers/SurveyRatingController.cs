using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CarRecommendationApp.Data;
using CarRecommendationApp.Models;

namespace CarRecommendationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyRatingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SurveyRatingController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/SurveyRating
        [HttpGet]
        public IActionResult GetSurveyRatings()
        {
            var ratings = _context.SurveyRatings.ToList();

            return Ok(ratings);
        }

        // GET: api/SurveyRating/user/1
        [HttpGet("user/{userId}")]
        public IActionResult GetUserRatings(int userId)
        {
            var ratings = _context.SurveyRatings
                .Where(x => x.SurveyUserId == userId)
                .ToList();

            return Ok(ratings);
        }

        // POST: api/SurveyRating
        [HttpPost]
        public IActionResult PostSurveyRating([FromBody] SurveyRating rating)
        {
            var existingRating =
                _context.SurveyRatings
                .FirstOrDefault(x =>
                    x.SurveyUserId == rating.SurveyUserId &&
                    x.CarId == rating.CarId);

            // Ako rating već postoji → update
            if (existingRating != null)
            {
                existingRating.Score = rating.Score;

                _context.SaveChanges();

                return Ok(existingRating);
            }

            // Novi rating
            rating.CreatedAt = DateTime.Now;

            _context.SurveyRatings.Add(rating);

            _context.SaveChanges();

            return Ok(rating);
        }
    }
}