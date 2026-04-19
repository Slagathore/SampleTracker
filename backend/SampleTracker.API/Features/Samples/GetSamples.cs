using MediatR;
using Microsoft.EntityFrameworkCore;
using SampleTracker.API.Data;
using SampleTracker.API.Models;
using SampleTracker.API.Models.DTOs;

namespace SampleTracker.API.Features.Samples;

public record GetSamplesQuery(string? StatusFilter) : IRequest<List<SampleResponse>>;

public class GetSamplesHandler : IRequestHandler<GetSamplesQuery, List<SampleResponse>>
{
    private readonly AppDbContext _db;
    public GetSamplesHandler(AppDbContext db) => _db = db;

    public async Task<List<SampleResponse>> Handle(
        GetSamplesQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Samples
            .Include(s => s.Method)
            .Include(s => s.Analyst)
            .AsQueryable();

        if (request.StatusFilter is not null &&
            Enum.TryParse<SampleStatus>(request.StatusFilter, true, out var parsed))
            query = query.Where(s => s.Status == parsed);

        return await query
            .OrderByDescending(s => s.ReceivedAt)
            .Select(s => new SampleResponse(
                s.Id, s.SampleId, s.Matrix, s.CollectedAt,
                s.Status.ToString(),
                s.Method != null ? s.Method.Code : null,
                s.Analyst != null ? s.Analyst.DisplayName : null,
                s.Notes))
            .ToListAsync(cancellationToken);
    }
}
