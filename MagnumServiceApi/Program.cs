using Microsoft.EntityFrameworkCore;
using MagnumServiceApi.Data;
using MagnumServiceApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext según el entorno
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();

    // if (env.IsDevelopment())
    // {
    //     options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    // }
    // else
    // {
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    // }
});

// Add services to the container.
// 
builder.Services.AddControllers();

builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IMoveService, MoveService>();
builder.Services.AddScoped<IRoundService, RoundService>();

// Register Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080);
});

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
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