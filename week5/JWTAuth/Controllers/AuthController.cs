using JWTAuth.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTAuth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configurationSettings;

        public AuthController(IConfiguration config)
        {
            _configurationSettings = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel userInput)
        {
            if (IsValidUser(userInput))
            {
                var generatedToken = GenerateJwtToken(userInput.Username);
                return Ok(new { token = generatedToken });
            }

            return Unauthorized();
        }

        [Authorize]
        [HttpGet("secret")]
        public IActionResult SecretEndpoint()
        {
            return Ok("Authorized and can access protected endpoint.");
        }

        [Authorize]
        [HttpGet("data")]
        public IActionResult GetUserData()
        {
            var userName = User.Identity?.Name;
            return Ok($"This is protected data.{userName}!");
        }

        private bool IsValidUser(LoginModel credentials)
        {
            return credentials.Username == "adhi" && credentials.Password == "adhi@123";
        }

        private string GenerateJwtToken(string userName)
        {
            var userClaims = new[]
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, "User")
            };

            var jwtKey = _configurationSettings["Jwt:Key"]!;
            var jwtIssuer = _configurationSettings["Jwt:Issuer"]!;
            var jwtAudience = _configurationSettings["Jwt:Audience"]!;
            var jwtDurationMinutes = double.Parse(_configurationSettings["Jwt:DurationInMinutes"]!);

            var securityKeyBytes = Encoding.UTF8.GetBytes(jwtKey);
            var signingKey = new SymmetricSecurityKey(securityKeyBytes);
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(userClaims),
                Expires = DateTime.UtcNow.AddMinutes(jwtDurationMinutes),
                SigningCredentials = credentials,
                Issuer = jwtIssuer,
                Audience = jwtAudience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}