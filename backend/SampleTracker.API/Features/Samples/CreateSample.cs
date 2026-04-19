using MediatR;
using SampleTracker.API.Data;
using SampleTracker.API.Models;
using SampleTracker.API.Models.DTOs;

namespace SampleTracker.API.Features.Samples;

public record CreateSampleCommand(
    string SampleId,
    string Matrix,
    DateTime CollectedAt,
    int? MethodId,
    int? AnalystId,
    string? Notes
) : IRequest<SampleResponse>;

public class CreateSampleHandler : IRequestHandler<CreateSampleCommand, SampleResponse>
{
    private readonly AppDbContext _db;
    public CreateSampleHandler(AppDbContext db) => _db = db;

    public async Task<SampleResponse> Handle(
        CreateSampleCommand request, CancellationToken cancellationToken)
    {
        var sample = new Sample
        {
            SampleId    = request.SampleId,
            Matrix      = request.Matrix,
            CollectedAt = request.CollectedAt,
            MethodId    = request.MethodId,
            AnalystId   = request.AnalystId,
            Notes       = request.Notes
        };

        _db.Samples.Add(sample);
        await _db.SaveChangesAsync(cancellationToken);

        return new SampleResponse(
            sample.Id, sample.SampleId, sample.Matrix,
            sample.CollectedAt, sample.Status.ToString(),
            null, null, sample.Notes);
    }
}
