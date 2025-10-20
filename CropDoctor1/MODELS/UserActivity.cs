using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropDoctor1.Models
{
    public class UserActivity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty; // Initialized here

        // The null-forgiving operator (!) tells the compiler we know this will be populated by EF Core.
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string ActivityType { get; set; } = string.Empty; // Initialized here

        public string? Details { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }
    }
}