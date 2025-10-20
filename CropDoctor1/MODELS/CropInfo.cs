using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropDoctor1.Models
{
    public class CropInfo
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string? Name { get; set; }

        public string? ImageUrl { get; set; }

        // This is the property that WILL be mapped to the database column.
        // It is initialized to an empty string to prevent the nullability warning.
        public string PlantingSeasons_Db { get; set; } = string.Empty;

        // This is the second property that will be mapped to a database column.
        public string SuitableSoilTypes_Db { get; set; } = string.Empty;

        // This property provides a convenient List<string> for C# code.
        // It is NOT mapped to the database.
        [NotMapped]
        public List<string> PlantingSeasons
        {
            get => string.IsNullOrEmpty(PlantingSeasons_Db) ? new List<string>() : PlantingSeasons_Db.Split(',').ToList();
            set => PlantingSeasons_Db = string.Join(",", value);
        }

        // This property also provides a convenient List<string> for C# code.
        [NotMapped]
        public List<string> SuitableSoilTypes
        {
            get => string.IsNullOrEmpty(SuitableSoilTypes_Db) ? new List<string>() : SuitableSoilTypes_Db.Split(',').ToList();
            set => SuitableSoilTypes_Db = string.Join(",", value);
        }

        [MaxLength(50)]
        public string? TemperatureRange { get; set; }

        public string? MarketInsight { get; set; }

        public string? SowingGuide { get; set; }
    }
}