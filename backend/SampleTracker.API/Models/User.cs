namespace SampleTracker.API.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Analyst;
    public ICollection<Sample> AssignedSamples { get; set; } = new List<Sample>();
}

public enum UserRole { Analyst, Admin }
