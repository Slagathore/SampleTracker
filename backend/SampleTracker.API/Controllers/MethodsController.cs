using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleTracker.API.Data;

namespace SampleTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MethodsController : ControllerBase
{
    private readonly AppDbContext _db;
    public MethodsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _db.Methods.OrderBy(m => m.Code).ToListAsync());
}
