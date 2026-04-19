namespace SampleTracker.API.Models;

public class Sample
{
    public int Id { get; set; }
    public string SampleId { get; set; } = string.Empty;
    public string Matrix { get; set; } = string.Empty;
    public DateTime CollectedAt { get; set; }
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    public SampleStatus Status { get; set; } = SampleStatus.Received;
    public string? Notes { get; set; }

    public int? MethodId { get; set; }
    public AnalyticalMethod? Method { get; set; }

    public int? AnalystId { get; set; }
    public User? Analyst { get; set; }
}

public enum SampleStatus
{
    Received,
    InPrep,
    InAnalysis,
    QCReview,
    Complete,
    Rejected
}
