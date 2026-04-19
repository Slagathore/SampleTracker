namespace SampleTracker.API.Models.DTOs;

public record CreateSampleRequest(
    string SampleId,
    string Matrix,
    DateTime CollectedAt,
    int? MethodId,
    int? AnalystId,
    string? Notes
);

public record SampleResponse(
    int Id,
    string SampleId,
    string Matrix,
    DateTime CollectedAt,
    string Status,
    string? MethodCode,
    string? AnalystName,
    string? Notes
);
