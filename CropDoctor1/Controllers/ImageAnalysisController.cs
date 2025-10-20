using CropDoctor1.Models;
using CropDoctor1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

// ViewModels for this controller's data
// They are now corrected with initializers.
public class AIPrediction
{
    public string prediction { get; set; } = string.Empty;
    public double confidence { get; set; }
}
public class AnalysisResult
{
    public string PredictedLabel { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public DiseaseInfo? Details { get; set; }
}

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ImageAnalysisController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly DiseaseInfoService _diseaseInfoService;
    private readonly ActivityLogService _activityLogService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ImageAnalysisController> _logger;

    public ImageAnalysisController(
        IHttpClientFactory httpClientFactory,
        DiseaseInfoService diseaseInfoService,
        ActivityLogService activityLogService,
        UserManager<ApplicationUser> userManager,
        ILogger<ImageAnalysisController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _diseaseInfoService = diseaseInfoService;
        _activityLogService = activityLogService;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        try
        {
            var pythonApiServiceUrl = "http://127.0.0.1:8000/predict";
            var client = _httpClientFactory.CreateClient();
            using var content = new MultipartFormDataContent();
            content.Add(new StreamContent(file.OpenReadStream()), "file", file.FileName);

            var response = await client.PostAsync(pythonApiServiceUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("AI Service returned an error: {StatusCode}", response.StatusCode);
                return StatusCode(500, "Error calling AI service.");
            }
            var aiPrediction = await response.Content.ReadFromJsonAsync<AIPrediction>();
            if (aiPrediction == null) return StatusCode(500, "Failed to understand AI prediction.");

            var diseaseDetails = await _diseaseInfoService.GetByDiseaseKeyAsync(aiPrediction.prediction);

            var finalResult = new AnalysisResult
            {
                PredictedLabel = aiPrediction.prediction,
                Confidence = aiPrediction.confidence,
                Details = diseaseDetails
            };

            string logDetails = $"File: {file.FileName}, Prediction: {finalResult.PredictedLabel}, Confidence: {finalResult.Confidence:P2}";
            await _activityLogService.LogActivityAsync(user.Id, "ImageUpload", logDetails);

            return Ok(finalResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during image analysis.");
            return StatusCode(500, "An internal server error occurred.");
        }
    }
}