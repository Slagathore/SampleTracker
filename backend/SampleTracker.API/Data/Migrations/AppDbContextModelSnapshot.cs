using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SampleTracker.API.Data;

#nullable disable

namespace SampleTracker.API.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "10.0.6");

            modelBuilder.Entity("SampleTracker.API.Models.AnalyticalMethod", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");
                b.Property<string>("Code").IsRequired().HasColumnType("TEXT");
                b.Property<string>("Instrument").IsRequired().HasColumnType("TEXT");
                b.Property<string>("Name").IsRequired().HasColumnType("TEXT");
                b.HasKey("Id");
                b.ToTable("Methods");
                b.HasData(
                    new { Id = 1, Code = "EPA 200.8", Instrument = "ICP-MS",       Name = "Metals by ICP-MS" },
                    new { Id = 2, Code = "EPA 524.2", Instrument = "GC-MS",        Name = "VOCs by GC-MS"    },
                    new { Id = 3, Code = "EPA 8270D", Instrument = "GC-MS",        Name = "SVOCs by GC-MS"   },
                    new { Id = 4, Code = "EPA 300.0", Instrument = "IC",           Name = "Anions by IC"     },
                    new { Id = 5, Code = "SM 5310B",  Instrument = "TOC Analyzer", Name = "TOC"              });
            });

            modelBuilder.Entity("SampleTracker.API.Models.Sample", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");
                b.Property<int?>("AnalystId").HasColumnType("INTEGER");
                b.Property<DateTime>("CollectedAt").HasColumnType("TEXT");
                b.Property<int?>("MethodId").HasColumnType("INTEGER");
                b.Property<string>("Matrix").IsRequired().HasColumnType("TEXT");
                b.Property<string>("Notes").HasColumnType("TEXT");
                b.Property<DateTime>("ReceivedAt").HasColumnType("TEXT");
                b.Property<string>("SampleId").IsRequired().HasColumnType("TEXT");
                b.Property<int>("Status").HasColumnType("INTEGER");
                b.HasKey("Id");
                b.HasIndex("AnalystId");
                b.HasIndex("MethodId");
                b.ToTable("Samples");
            });

            modelBuilder.Entity("SampleTracker.API.Models.User", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");
                b.Property<string>("DisplayName").IsRequired().HasColumnType("TEXT");
                b.Property<string>("Email").IsRequired().HasColumnType("TEXT");
                b.Property<string>("PasswordHash").IsRequired().HasColumnType("TEXT");
                b.Property<int>("Role").HasColumnType("INTEGER");
                b.HasKey("Id");
                b.ToTable("Users");
            });

            modelBuilder.Entity("SampleTracker.API.Models.Sample", b =>
            {
                b.HasOne("SampleTracker.API.Models.User", "Analyst")
                    .WithMany("AssignedSamples")
                    .HasForeignKey("AnalystId");
                b.HasOne("SampleTracker.API.Models.AnalyticalMethod", "Method")
                    .WithMany("Samples")
                    .HasForeignKey("MethodId");
                b.Navigation("Analyst");
                b.Navigation("Method");
            });

            modelBuilder.Entity("SampleTracker.API.Models.AnalyticalMethod", b =>
                b.Navigation("Samples"));

            modelBuilder.Entity("SampleTracker.API.Models.User", b =>
                b.Navigation("AssignedSamples"));
#pragma warning restore 612, 618
        }
    }
}
