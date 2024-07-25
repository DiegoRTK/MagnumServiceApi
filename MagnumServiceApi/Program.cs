using MagnumServiceApi.Data;
using MagnumServiceApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext según el entorno
builder.Services.AddDbContext<AppDbContext>(
    (serviceProvider, options) =>
    {
        var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IMoveService, MoveService>();
builder.Services.AddScoped<IRoundService, RoundService>();

// Register Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Kestrel to listen on the port specified by Railway
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.ConfigureKestrel(options =>
{
     // Configura Kestrel para usar HTTPS
    options.ListenAnyIP(443, listenOptions =>
    {
        listenOptions.UseHttps();
    });

    // Configura Kestrel para usar HTTP (si es necesario)
    options.ListenAnyIP(int.Parse(port)); // O cualquier otro puerto que estés usando para HTTP
});

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
