using CarRecommendationApp.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// === CORS - Bolja konfiguracija ===
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",
            "http://localhost:5174",
            "http://127.0.0.1:5173",
            "http://127.0.0.1:5174",
            "https://ivaarlovic.github.io"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();   // važno za auth kasnije
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 0)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure()
    ));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// === Middleware redoslijed JE BITAN ===
    app.UseSwagger();
    app.UseSwaggerUI();


app.UseCors("AllowFrontend"); 

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();