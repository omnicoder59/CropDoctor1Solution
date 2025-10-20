using System.ComponentModel.DataAnnotations;

namespace CropDoctor1.Models
{
    public class DiseaseInfo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string DiseaseKey { get; set; } = string.Empty; // Initialized here

        [MaxLength(100)]
        public string? PlantName { get; set; }

        [MaxLength(150)]
        public string? DiseaseName { get; set; }

        public string? Cause { get; set; }

        public string? RecommendedSolution { get; set; }
    }
}