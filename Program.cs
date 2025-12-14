using Healthcare.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register App Services
// Register App Services
builder.Services.AddScoped<Healthcare.Api.Services.IOcrService, Healthcare.Api.Services.OcrService>();
builder.Services.AddScoped<Healthcare.Api.Services.IAiService, Healthcare.Api.Services.AiService>();

builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value ?? "ThisIsAVeryLongSecretKeyForHealthPulseAppToEnsureSecurityAndComplianceWithHMACSHA512Algorithm_MustBeAtLeast64BytesLong_1234567890")),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddDbContext<Healthcare.Api.Data.AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

// Initial Seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try 
    {
        var context = services.GetRequiredService<Healthcare.Api.Data.AppDbContext>();
        context.Database.EnsureCreated();
        
        // Check if we have users
        if (!context.Users.Any())
        {
            Console.WriteLine("Seeding Default Users...");
            var users = new List<Healthcare.Core.Entities.User>
            {
                new Healthcare.Core.Entities.User 
                { 
                    FullName = "Admin User", 
                    Email = "admin@hlc.com", 
                    Role = Healthcare.Core.Entities.UserRole.Admin,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123")
                },
                new Healthcare.Core.Entities.User 
                { 
                    FullName = "Dr. House", 
                    Email = "doctor@hlc.com", 
                    Role = Healthcare.Core.Entities.UserRole.Provider,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("doctor123")
                },
                new Healthcare.Core.Entities.User 
                { 
                    FullName = "Nurse Joy", 
                    Email = "nurse@hlc.com", 
                    Role = Healthcare.Core.Entities.UserRole.Clinical,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("nurse123")
                },
                new Healthcare.Core.Entities.User 
                { 
                    FullName = "Pharma Joe", 
                    Email = "pharma@hlc.com", 
                    Role = Healthcare.Core.Entities.UserRole.Pharmacy,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("pharma123")
                },
                 new Healthcare.Core.Entities.User 
                { 
                    FullName = "Receptionist Pam", 
                    Email = "reception@hlc.com", 
                    Role = Healthcare.Core.Entities.UserRole.Reception,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("reception123")
                }
            };
            
            context.Users.AddRange(users);
            context.SaveChanges();
            Console.WriteLine("Seeding Complete!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred seeding the DB: {ex.Message}");
    }
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
