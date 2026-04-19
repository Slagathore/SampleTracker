using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SampleTracker.API.Data;

namespace SampleTracker.API.Features.Samples;

public record ExportSamplesCsvQuery : IRequest<byte[]>;

public class ExportSamplesCsvHandler : IRequestHandler<ExportSamplesCsvQuery, byte[]>
{
    private readonly AppDbContext _db;
    public ExportSamplesCsvHandler(AppDbContext db) => _db = db;

    public async Task<byte[]> Handle(
        ExportSamplesCsvQuery request, CancellationToken cancellationToken)
    {
        var samples = await _db.Samples
            .Include(s => s.Method)
            .Include(s => s.Analyst)
            .OrderByDescending(s => s.ReceivedAt)
            .ToListAsync(cancellationToken);

        var sb = new StringBuilder();
        sb.AppendLine("SampleId,Matrix,CollectedAt,ReceivedAt,Status,Method,Analyst,Notes");

        foreach (var s in samples)
        {
            sb.AppendLine(string.Join(',',
                Escape(s.SampleId),
                Escape(s.Matrix),
                s.CollectedAt.ToString("yyyy-MM-dd"),
                s.ReceivedAt.ToString("yyyy-MM-dd"),
                s.Status.ToString(),
                Escape(s.Method?.Code ?? ""),
                Escape(s.Analyst?.DisplayName ?? ""),
                Escape(s.Notes ?? "")));
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private static string Escape(string val) =>
        val.Contains(',') || val.Contains('"') || val.Contains('\n')
            ? $"\"{val.Replace("\"", "\"\"")}\""
            : val;
}
