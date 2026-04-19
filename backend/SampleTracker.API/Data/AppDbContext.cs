using Microsoft.EntityFrameworkCore;
using SampleTracker.API.Models;

namespace SampleTracker.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlite("Data Source=sampletracker.db");
    }

    public DbSet<Sample> Samples => Set<Sample>();
    public DbSet<AnalyticalMethod> Methods => Set<AnalyticalMethod>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<AnalyticalMethod>().HasData(
            new AnalyticalMethod { Id = 1, Code = "EPA 200.8", Name = "Metals by ICP-MS",  Instrument = "ICP-MS"       },
            new AnalyticalMethod { Id = 2, Code = "EPA 524.2", Name = "VOCs by GC-MS",      Instrument = "GC-MS"        },
            new AnalyticalMethod { Id = 3, Code = "EPA 8270D", Name = "SVOCs by GC-MS",     Instrument = "GC-MS"        },
            new AnalyticalMethod { Id = 4, Code = "EPA 300.0", Name = "Anions by IC",        Instrument = "IC"           },
            new AnalyticalMethod { Id = 5, Code = "SM 5310B",  Name = "TOC",                 Instrument = "TOC Analyzer" }
        );
    }
}
