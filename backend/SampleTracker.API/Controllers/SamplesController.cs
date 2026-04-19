using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleTracker.API.Features.Samples;
using SampleTracker.API.Models.DTOs;

namespace SampleTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SamplesController : ControllerBase
{
    private readonly ISender _bus;
    public SamplesController(ISender bus) => _bus = bus;

    [HttpGet]
    public async Task<ActionResult<List<SampleResponse>>> GetAll([FromQuery] string? status)
        => Ok(await _bus.Send(new GetSamplesQuery(status)));

    [HttpPost]
    public async Task<ActionResult<SampleResponse>> Create(CreateSampleRequest req)
    {
        var result = await _bus.Send(new CreateSampleCommand(
            req.SampleId, req.Matrix, req.CollectedAt,
            req.MethodId, req.AnalystId, req.Notes));

        return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string newStatus)
    {
        var success = await _bus.Send(new UpdateStatusCommand(id, newStatus));
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        // TODO: add DeleteSampleCommand handler
        return NoContent();
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
        => Ok(await _bus.Send(new GetSampleStatsQuery()));

    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var csv = await _bus.Send(new ExportSamplesCsvQuery());
        return File(csv, "text/csv", $"samples-{DateTime.UtcNow:yyyyMMdd}.csv");
    }
}
