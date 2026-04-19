using MediatR;
using Microsoft.EntityFrameworkCore;
using SampleTracker.API.Data;

namespace SampleTracker.API.Features.Samples;

public record GetSampleStatsQuery : IRequest<List<StatusStat>>;
public record StatusStat(string Status, int Count);

public class GetSampleStatsHandler : IRequestHandler<GetSampleStatsQuery, List<StatusStat>>
{
    private readonly AppDbContext _db;
    public GetSampleStatsHandler(AppDbContext db) => _db = db;

    public async Task<List<StatusStat>> Handle(
        GetSampleStatsQuery request, CancellationToken cancellationToken)
    {
        return await _db.Samples
            .GroupBy(s => s.Status)
            .Select(g => new StatusStat(g.Key.ToString(), g.Count()))
            .ToListAsync(cancellationToken);
    }
}
