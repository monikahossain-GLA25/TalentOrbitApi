using Microsoft.EntityFrameworkCore;
using TalentOrbitApi.Data;
using TalentOrbitApi.Services;
using TalentOrbitApi.Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();


builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["traceId"] =
            context.HttpContext.TraceIdentifier;

        context.ProblemDetails.Extensions["timestamp"] =
            DateTimeOffset.UtcNow;
    };
});

var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' was not found.");


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<IMentorService, MentorService>();
var app = builder.Build();


app.UseExceptionHandler();


app.UseStatusCodePages();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
