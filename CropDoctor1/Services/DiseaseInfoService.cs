using CropDoctor1.Data;
using CropDoctor1.Models;
using Microsoft.EntityFrameworkCore;

namespace CropDoctor1.Services
{
    public class DiseaseInfoService
    {
        private readonly ApplicationDbContext _context;

        public DiseaseInfoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DiseaseInfo?> GetByDiseaseKeyAsync(string key)
        {
            return await _context.DiseaseInfos
             .FirstOrDefaultAsync(d => d.DiseaseKey.ToLower() == key.ToLower());
        }
    }
}