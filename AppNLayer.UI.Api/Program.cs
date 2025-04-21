using AppNLayer.DAL.Repositories;
using AppNLayer.DAL;
using Microsoft.EntityFrameworkCore;
using AppNLayer.BLL.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


//CORS
const string AllowAllCorsPolicy = "AllowAllOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowAllCorsPolicy,
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod();

                      });
});


//connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

builder.Services.AddDbContext<AppNLayerDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    }));
builder.Services.AddScoped<IDataAccess, DataAccess>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBookService, BookService>();
//Reflection

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "AppNLayer API",
        Version = "v1",
        Description = "API for managing Books and Categories"
    });
});


builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<AppNLayerDbContext>();

var app = builder.Build();
app.UseCors(AllowAllCorsPolicy);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "AppNLayer API V1");
    });
    app.UseDeveloperExceptionPage();
}

app.MapControllers();
app.UseHttpsRedirection();
app.MapIdentityApi<IdentityUser>();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppNLayerDbContext>();
    try
    {
        Console.WriteLine("Attempting to apply migrations...");
        dbContext.Database.Migrate(); // Applies pending migrations
        Console.WriteLine("Migrations applied successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error applying migrations: {ex.Message}");
        // Log the full exception details
    }

}


app.Run();
