using CarRecommendationApp.Data;
using CarRecommendationApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarRecommendationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarViewController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CarViewController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost] 
        public IActionResult AddView([FromBody] CarView carView)
        {
            carView.ViewedAt = DateTime.Now;

            _context.CarViews.Add(carView);
            _context.SaveChanges();

            return Ok(carView);
        }
    }
}
