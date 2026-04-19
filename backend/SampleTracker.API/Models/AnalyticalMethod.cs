namespace SampleTracker.API.Models;

public class AnalyticalMethod
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Instrument { get; set; } = string.Empty;
    public ICollection<Sample> Samples { get; set; } = new List<Sample>();
}
