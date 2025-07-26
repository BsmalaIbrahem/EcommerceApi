using ApplicationLayer.DTOs;
using InfrastructureLayer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PresentationLayer.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("api/Identity/[controller]/[action]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        public AccountController(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpPost]
        public async Task<IActionResult> Regiser(RegisterRequest request)
        {
            var existingUserByEmail = await _userManager.FindByEmailAsync(request.Email);
            if (existingUserByEmail != null)
            {
                return Json(new { success = false, message = "Email already exists." });
            }
            var existingUserByUsername = await _userManager.FindByNameAsync(request.UserName);
            if (existingUserByUsername != null)
            {
                return Json(new { success = false, message = "Username already exists." });
            }

            var user = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                Email = request.Email
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            
            if (!result.Succeeded)
            {
                return Json(new { success = false, message = "Registration failed.", errors = result.Errors });
            }
            await _userManager.AddToRoleAsync(user, "User");
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, token = token }, Request.Scheme);
            await _emailSender.SendEmailAsync(request.Email, "Confirm your email", $"Please confirm your account by clicking this link: <a href='{confirmationLink}'>link</a>");
            return Json(new { success = true, message = "Confirm email sended" });
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is not null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    return Ok("Confirm Email Successfully");
                }
                else
                {
                    return BadRequest($"{String.Join(",", result.Errors)}");
                }
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.UseNameOrEmail);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(request.UseNameOrEmail);
                if (user == null)
                {
                    return Json(new { success = false, message = "email or user name is invalid" });
                }
            }

            var result = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!result)
            {
                return Json(new { success = false, message = "Password is invalid." });
            }

            if(user.EmailConfirmed == false)
            {
                return Json(new { success = false, message = "Email is not confirmed." });
            }
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, string.Join(",", roles)),
            };
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("EraaSoft514##EraaSoft514##EraaSoft514##"));

            var signInCredential = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer : "https://localhost:7121",
                audience : "https://localhost:4200,https://localhost:5000",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: signInCredential

            );

            return Json(new { 
                success = false, 
                message = "Login functionality not implemented yet.", 
                data = new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    user = new
                    {
                        user.Id,
                        user.UserName,
                        user.Email,
                        user.FirstName,
                        user.LastName,
                        roles = roles
                    }
                }
            });
        }

        


    }
}
