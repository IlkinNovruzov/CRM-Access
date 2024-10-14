using CRM_Access.Models;
using CRM_Access.DTO;
using CRM_Access.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using ConstructionAPI.Services;
using System.Net.Mail;
namespace CRM_Access.Controllers.Admin;

[EnableCors("AllowSpecificOrigin")]
[Route("api/admin/[controller]")]
[ApiController]
public class UserController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, AppDbContext context, IEmailService emailService, SignInManager<AppUser> signInManager, TokenService tokenService) : ControllerBase
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly RoleManager<AppRole> _roleManager = roleManager;
    private readonly SignInManager<AppUser> _signInManager = signInManager;
    private readonly AppDbContext _context = context;
    private readonly IEmailService _emailService = emailService;
    private readonly TokenService _tokenService = tokenService;

    [HttpGet]
    public async Task<ActionResult<List<IdentityUser>>> GetUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        return Ok(users);
    }


    [HttpPost("email")]
    public async Task<IActionResult> PostEmail([FromBody] string email)
    {
        var emailSent = await SendEmail(email);

        if (emailSent)
        {
            return Ok("Email sent successfully.");
        }
        else
        {
            return StatusCode(500, $"Error sending email to: {email}");
        }
    }

    [HttpPost("emails")]
    public async Task<IActionResult> PostEmails([FromBody] List<string> emails)
    {
        if (emails == null || emails.Count == 0)
        {
            return BadRequest("No emails provided.");
        }

        foreach (var email in emails)
        {
            var emailSent = await SendEmail(email);
            if (!emailSent)
            {
                return StatusCode(500, $"Error sending email to: {email}");
            }
        }

        return Ok("All emails sent successfully.");
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new AppUser
        {
            UserName = model.Email,
            Email = model.Email,
            Name = model.Name,
            Surname = model.Surname,
            PhoneNumber = model.Phone,
            CompanyDomain = model.CompanyDomain,
            CompanyName = model.CompanyName,
            ImageUrl = "jjhhhhj",
            EmailConfirmed = true
        };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest(ModelState);
            }


            //await _signInManager.SignInAsync(user, isPersistent: false);
            return Ok("Register is successfully");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return BadRequest(ModelState);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return NotFound("User not found");
        }
        //if (!await _userManager.IsEmailConfirmedAsync(user))
        //{
        //    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //    var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        //    var confirmationLink = $"https://itb-blog.vercel.app/confirm-email?userId={user.Id}&token={encodedToken}";
        //    try
        //    {
        //        await _emailService.SendEmailAsync(model.Email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception
        //        return StatusCode(500, "Error sending confirmation email." + ex.Message);
        //    }

        //    return BadRequest("Email not confirmed. Confirmation email has been sent.");
        //}
        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            var jwtToken = await _tokenService.GenerateToken(user);
            return Ok(jwtToken);
        }

        if (result.IsLockedOut)
        {
            return BadRequest("User account locked out.");
        }

        return BadRequest("Invalid login attempt.");
    }

    [HttpGet("confirm-email")]
    public IActionResult ConfirmEmail(string token)
    {
        if (token == null)
        {
            return BadRequest("Invalid email confirmation request.");
        }

        //var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);
        //if (user == null)
        //{
        //    return BadRequest("Invalid email confirmation request.");
        //}
        //var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        //var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
        if (token == "jjj")
        {
            return Ok("Email confirmed successfully.");
        }

        return BadRequest("Error confirming your email.");
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser([FromRoute] int id)
    {
        //var user = await _tokenService.ValidateTokenAndGetUserAsync(token);

        //if (user == null) return Unauthorized(new { Message = "Invalid token" });

        //if (!await _userManager.IsInRoleAsync(user, "admin")) return Forbid();

        var removeUser = await _userManager.Users
            .SingleOrDefaultAsync(u => u.Id == id);

        if (removeUser == null) return NotFound(new { Message = "User Not Found" });

        //_context.Products.RemoveRange(removeUser.Products);
        //_context.Shops.RemoveRange(removeUser.Shops);
        //_context.Likes.RemoveRange(removeUser.Likes);
        //_context.SavedBlogs.RemoveRange(removeUser.SavedBlogs);

        var result = await _userManager.DeleteAsync(removeUser);
        if (!result.Succeeded) return BadRequest(result.Errors);

        await _context.SaveChangesAsync();

        return Ok(new { Message = "User deleted successfully" });
    }
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return BadRequest("Email is required.");
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        if (!(await _userManager.IsEmailConfirmedAsync(user)))
        {
            return BadRequest("Email is not confirmed. Please confirm your email before requesting a password reset.");
        }
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var confirmationLink = $"http://localhost:5173/reset-password?userId={user.Id}&token={encodedToken}";

        try
        {
            await _emailService.SendEmailAsync(email, "Reset Password", $"Please reset your password by <a href='{confirmationLink}'>clicking here</a>.");
        }
        catch (SmtpException ex)
        {
            return StatusCode(500, $"Error sending reset email: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error sending reset email: {ex.Message}");
        }

        return Ok("If the email is registered, a password reset link will be sent to the email address.");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
   

        var user = await _userManager.FindByIdAsync(model.UserId.ToString());
        if (user == null)
        {
            return BadRequest("Invalid email address.");
        }

        if (model.Password != model.ConfirmPassword)
        {
            return BadRequest("The password and confirmation password do not match.");
        }
        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
        var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.Password);
        if (result.Succeeded)
        {
            return Ok("Your password has been reset successfully.");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return BadRequest(ModelState);
    }

    //[HttpPost("{userId}")]
    //public async Task<IActionResult> AssignRole([FromRoute] int userId, [FromBody] List<AssignRoleDTO> list, [FromHeader(Name = "Authorization")] string token)
    //{
    //    var user = await _tokenService.ValidateTokenAndGetUserAsync(token);

    //    if (user == null) return Unauthorized(new { Message = "Invalid token" });

    //    if (!await _userManager.IsInRoleAsync(user, "admin")) return Forbid();

    //    var targetUser = await _userManager.FindByIdAsync(userId.ToString());
    //    if (targetUser == null) return NotFound(new { Message = "User Not Found" });

    //    foreach (var item in list)
    //    {
    //        IdentityResult result;
    //        if (item.Status)
    //        {
    //            result = await _userManager.AddToRoleAsync(targetUser, item.Name);
    //        }
    //        else
    //        {
    //            result = await _userManager.RemoveFromRoleAsync(targetUser, item.Name);
    //        }

    //        if (!result.Succeeded)
    //        {
    //            return BadRequest(result.Errors);
    //        }
    //    }

    //    return Ok(new { Message = "Roles updated successfully" });
    //}
    private async Task<bool> SendEmail(string email)
    {
        var token = _tokenService.GenerateEmailToken(email);
        var confirmationLink = $"http://localhost:5173/email-confirmation?token={token}";

        try
        {
            await _emailService.SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");
            return true;
        }
        catch (Exception ex)
        {
            // Hata loglanabilir (örneğin, _logger.LogError(ex, "Email gönderme hatası"))
            return false;
        }
    }
    #region Role

    [HttpGet("roles")]
    public IActionResult GetRoles()
    {
        var roles = _roleManager.Roles.ToList();
        return Ok(roles);
    }

    [HttpPost("role/create")]
    public async Task<IActionResult> AddRole([FromBody] string role)
    {
        var appRole = new AppRole
        {
            Name = role
        };

        var result = await _roleManager.CreateAsync(appRole);
        if (result.Succeeded)
        {
            return CreatedAtAction(nameof(GetRoles), null); // HTTP 201 Created
        }
        else
        {
            return BadRequest(result.Errors); // HTTP 400 Bad Request
        }
    }

    [HttpPut("role/edit/{id}")]
    public async Task<IActionResult> EditRole([FromRoute] int id, [FromBody] string roleName)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role == null)
        {
            return NotFound($"Role with ID {id} not found");
        }

        role.Name = roleName;

        var result = await _roleManager.UpdateAsync(role);
        if (result.Succeeded)
        {
            return Ok();
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }


    [HttpDelete("role/delete/{id}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role == null)
        {
            return NotFound("Role not found.");
        }

        var result = await _roleManager.DeleteAsync(role);
        if (result.Succeeded)
        {
            return Ok("Role removed.");
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }

    #endregion
}
