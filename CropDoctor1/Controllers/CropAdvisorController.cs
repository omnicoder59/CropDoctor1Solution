using CropDoctor1.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CropAdvisorController : ControllerBase
{
    private readonly CropInfoService _cropInfoService;

    public CropAdvisorController(CropInfoService cropInfoService)
    {
        _cropInfoService = cropInfoService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchCrops([FromQuery] string? season, [FromQuery] string? soil)
    {
        var results = await _cropInfoService.SearchCropsAsync(season, soil);
        return Ok(results);
    }
}