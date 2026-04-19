using MediatR;
using SampleTracker.API.Data;
using SampleTracker.API.Models;

namespace SampleTracker.API.Features.Samples;

public record UpdateStatusCommand(int SampleId, string NewStatus) : IRequest<bool>;

public class UpdateStatusHandler : IRequestHandler<UpdateStatusCommand, bool>
{
    private readonly AppDbContext _db;
    public UpdateStatusHandler(AppDbContext db) => _db = db;

    public async Task<bool> Handle(
        UpdateStatusCommand request, CancellationToken cancellationToken)
    {
        var sample = await _db.Samples.FindAsync(
            new object[] { request.SampleId }, cancellationToken);

        if (sample is null) return false;
        if (!Enum.TryParse<SampleStatus>(request.NewStatus, true, out var parsed)) return false;

        sample.Status = parsed;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
