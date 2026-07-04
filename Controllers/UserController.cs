using CarRecommendationApp.Data;
using CarRecommendationApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;

namespace CarRecommendationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        //GET
        [HttpGet]
        public IActionResult GetUser()
        {
            var users = _context.Users.ToList();
            return Ok(users);
        }

        //POST
        [HttpPost]
        public IActionResult AddUser([FromBody] User user)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);
            if(existingUser != null)
            {
                return BadRequest("Korisnik s ovim emailom već postoji!");
            }

            //Hashiranje lozinke prije spremanja
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new {message = "Korisnik uspješno registriran!"});
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginData)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == loginData.Email);
            if (user == null)
            {
                return Unauthorized("Email nije pronađen.");
            }

            //Provjera hashirane lozinke
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginData.Password, user.PasswordHash);
            if(!isPasswordValid)
            {
                return Unauthorized("Netočna lozinka.");
            }

            return Ok(new { message = "Prijava uspjesna", userId = user.Id, username = user.Username });
        }



    }
}
