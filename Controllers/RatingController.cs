using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CarRecommendationApp.Data;
using CarRecommendationApp.Models;


namespace CarRecommendationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RatingController(AppDbContext context)
        {
            _context = context; 
        }

        //GET
        [HttpGet]
        public IActionResult GetRatings()
        {
            var ratings = _context.Ratings.ToList();
            return Ok(ratings);
        }

        //POST

        [HttpPost]
        public IActionResult PostRatings([FromBody] Rating rating)
        {

            var existingRating = 
                _context.Ratings
                .FirstOrDefault(x => 
                    x.UserId == rating.UserId &&
                    x.CarId == rating.CarId);

            if (existingRating != null)
            {
                existingRating.Score = rating.Score;
                _context.SaveChanges();
                return Ok(existingRating);
            }

                _context.Ratings.Add(rating);
            _context.SaveChanges();

            return Ok(rating);
        }


    }
}
