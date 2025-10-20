using CropDoctor1.Data;
using CropDoctor1.Models;
using Microsoft.EntityFrameworkCore;

namespace CropDoctor1.Services // Ensure this namespace is correct
{
    public class CropInfoService
    {
        private readonly ApplicationDbContext _context;

        public CropInfoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CropInfo>> SearchCropsAsync(string? season, string? soilType)
        {
            var query = _context.CropInfos.AsQueryable();

            if (!string.IsNullOrEmpty(season))
            {
                // Query against the database-mapped string field
                query = query.Where(c => c.PlantingSeasons_Db.Contains(season));
            }

            if (!string.IsNullOrEmpty(soilType))
            {
                // Query against the database-mapped string field
                query = query.Where(c => c.SuitableSoilTypes_Db.Contains(soilType));
            }

            return await query.ToListAsync();
        }
    }
}