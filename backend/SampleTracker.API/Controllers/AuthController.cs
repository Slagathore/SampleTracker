using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleTracker.API.Data;
using SampleTracker.API.Models;
using SampleTracker.API.Models.DTOs;
using SampleTracker.API.Services;

namespace SampleTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly TokenService _tokens;

    public AuthController(AppDbContext db, TokenService tokens)
    {
        _db = db;
        _tokens = tokens;
    }

    [HttpPost("register")]
    public async Task<ActionResult<LoginResponse>> Register(RegisterRequest req)
    {
        if (await _db.Users.AnyAsync(u => u.Email == req.Email))
            return BadRequest("Email already registered.");

        var user = new User
        {
            Email        = req.Email,
            DisplayName  = req.DisplayName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(new LoginResponse(
            _tokens.CreateToken(user), user.DisplayName, user.Role.ToString()));
    }

    [HttpPost("guest")]
    public async Task<ActionResult<LoginResponse>> Guest()
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == "guest@sampletracker.demo");
        if (user is null) return StatusCode(503, "Guest account not available.");
        return Ok(new LoginResponse(
            _tokens.CreateToken(user), user.DisplayName, user.Role.ToString()));
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest req)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == req.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials.");

        return Ok(new LoginResponse(
            _tokens.CreateToken(user), user.DisplayName, user.Role.ToString()));
    }
}
