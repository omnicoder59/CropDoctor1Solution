using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;

// ViewModels for News API response
public class NewsArticle
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? UrlToImage { get; set; }
}
public class NewsApiResponse
{
    public List<NewsArticle>? Articles { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public NewsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> GetAgricultureNews()
    {
        // IMPORTANT: Add "NewsApiKey": "YOUR_KEY" to your appsettings.json
        var apiKey = _configuration["NewsApiKey:ApiKey"];
        if (string.IsNullOrEmpty(apiKey)) return StatusCode(500, "News API Key not configured.");

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("CropDoctorApp", "1.0"));
        var newsApiUrl = $"https://newsapi.org/v2/everything?q=agriculture&sortBy=publishedAt&language=en&apiKey={apiKey}";

        try
        {
            var newsResult = await client.GetFromJsonAsync<NewsApiResponse>(newsApiUrl);
            return Ok(newsResult?.Articles?.Take(12));
        }
        catch (Exception)
        {
            return StatusCode(500, "Failed to fetch news please check the model api is running or not: ");
        }
    }
}