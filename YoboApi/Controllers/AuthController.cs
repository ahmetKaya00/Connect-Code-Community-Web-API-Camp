using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using YoboApi.Models;
using YoboApi.Dtos;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
namespace YoboApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IConfiguration _config;

    public AuthController(UserManager<AppUser> userManager,SignInManager<AppUser>signInManager,IConfiguration config)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult>Register([FromBody]RegisterRequest dto)
    {
        var exists = await _userManager.FindByEmailAsync(dto.Email);
        if(exists is not null)
           return Conflict(new{message = "Bu e-posta zaten kayıtlı."});

        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName
        };
        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            return BadRequest(new {errors = result.Errors.Select(e => e.Description)});
        }

        var token = GenerateToken(user);
        return Ok(token);
    }

    [HttpPost("login")]
    public async Task<IActionResult>Login([FromBody]LoginRequest dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if(user is null)
           return Unauthorized(new{message="Kullanıcı bulunamadı."});
        
        var check = await _signInManager.CheckPasswordSignInAsync(user,dto.Password,lockoutOnFailure:false);
        if(!check.Succeeded)
           return Unauthorized(new {message="E-posta veya Parola hatalı."});
        
        var token = GenerateToken(user);
        return Ok(token);
    }

    private object GenerateToken(AppUser user)
    {
        var jwt = _config.GetSection("JwtSettings");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));

        var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? "")
        };

        var expires = DateTime.UtcNow.AddMinutes(double.Parse(jwt["ExpirtyMinutes"]!));

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new YoboApi.Dtos.AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expires,
            Email = user.Email ?? "",
            FullName = user.FullName
        };
    }
}