using CropDoctor1.Data;
using CropDoctor1.Models;

namespace CropDoctor1.Services
{
    public class ActivityLogService
    {
        private readonly ApplicationDbContext _context;

        public ActivityLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogActivityAsync(string userId, string activityType, string? details = null)
        {
            if (string.IsNullOrEmpty(userId)) return;

            var activity = new UserActivity
            {
                UserId = userId,
                ActivityType = activityType,
                Details = details,
                Timestamp = DateTime.UtcNow
            };

            await _context.UserActivities.AddAsync(activity);
            await _context.SaveChangesAsync();
        }
    }
}