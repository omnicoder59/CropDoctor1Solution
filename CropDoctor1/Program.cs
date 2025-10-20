using CropDoctor1.Data;
using CropDoctor1.Models;
using CropDoctor1.Services; // <-- THIS LINE WAS ADDED
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Configure Services ---

// Add the DbContext and connect it to the SQL Server using the connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add ASP.NET Core Identity for user management
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Add support for API controllers
builder.Services.AddControllers();

// Add HttpClient for calling external APIs
builder.Services.AddHttpClient();

// Register your custom application services
builder.Services.AddScoped<DiseaseInfoService>();
builder.Services.AddScoped<CropInfoService>();
builder.Services.AddScoped<ActivityLogService>();

// Add Swagger for API testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- 2. Build the Application ---
var app = builder.Build();

// --- 3. Configure the HTTP Request Pipeline ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// This is where you will add middleware for serving static files later
// app.UseDefaultFiles();
// app.UseStaticFiles();

app.UseDefaultFiles(); // This makes index.html the default start page for the root URL
app.UseStaticFiles();  // This enables serving files (like HTML, CSS, JS) from the wwwroot folder
app.UseCors();         // Enables Cross-Origin requests if needed during development


app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// This calls your seeder to populate the database on the first run
DbSeeder.Seed(app);

app.Run();