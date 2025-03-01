using DataLogger.DTOs;
using DataLoggerDatabase.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DataLogger.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration config) : ControllerBase
    {
        private readonly AppSettings appSettings = config.Get<AppSettings>() ?? throw new InvalidOperationException("Missing AppSetting.SecretKey");

        //[Authorize]
        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser(LoginDto registerRequest)
        {
            //var currentUser = await userManager.GetUserAsync(User);
            //if (currentUser == null || !currentUser.IsAdmin) return Unauthorized();

            //if (registerRequest == null) return BadRequest("Invalid register request");

            var user = new AppUser { UserName = registerRequest.Username, };

            var result = await userManager.CreateAsync(user, registerRequest.Password);

            if (result.Succeeded) return Ok();

            return BadRequest("Invalid register request");
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto loginRequest)
        {
            var user = await userManager.FindByNameAsync(loginRequest.Username);
            if (user == null) return BadRequest("Invalid credentials");

            var result = await signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);
            if (result.Succeeded)
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.SecretKey));
                var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity([new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString())]),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = signingCredentials
                };

                var handler = new JwtSecurityTokenHandler();
                var token = handler.CreateToken(tokenDescriptor);
                var tokenString = handler.WriteToken(token);

                return Ok(new { accessToken = tokenString });
            }

            return BadRequest("Invalid credentials");
        }
    }
}
