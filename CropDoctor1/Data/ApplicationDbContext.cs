using CropDoctor1.Models; // Make sure this namespace is correct
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CropDoctor1.Data // Make sure this namespace is correct
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // These DbSet properties tell EF Core which of our models should become tables.
        public DbSet<DiseaseInfo> DiseaseInfos { get; set; }
        public DbSet<CropInfo> CropInfos { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // This line is required for ASP.NET Core Identity tables to be configured correctly.
            // It MUST be the first line in this method.
            base.OnModelCreating(modelBuilder);

            // --- Configuration for the CropInfo model ---
            // We need to tell EF Core how to handle our List<string> properties.

            // Ignore the List<string> properties so EF Core doesn't try to create columns for them.
            modelBuilder.Entity<CropInfo>().Ignore(c => c.PlantingSeasons);
            modelBuilder.Entity<CropInfo>().Ignore(c => c.SuitableSoilTypes);

            // Map the string properties to the database columns instead.
            modelBuilder.Entity<CropInfo>().Property(c => c.PlantingSeasons_Db).HasColumnName("PlantingSeasons");
            modelBuilder.Entity<CropInfo>().Property(c => c.SuitableSoilTypes_Db).HasColumnName("SuitableSoilTypes");
        }
    }
}