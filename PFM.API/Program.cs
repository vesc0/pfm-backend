using PFM.Infrastructure;
using PFM.Application;
using System.Text.Json.Serialization;
using PFM.API.Serialization;
using Microsoft.EntityFrameworkCore; 
using PFM.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddYamlFile("auto-categorize-rules.yml", optional: false, reloadOnChange: true);

// Register services
builder.Services
    .AddInfrastructure(builder.Configuration)  // EF Core, DbContext, repos
    .AddApplication();                           // MediatR, validators, handlers

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Angular dev server
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add MVC Controller support
builder.Services
    .AddControllers(options =>
        {
            options.Filters.Add<PFM.API.Filters.ApiExceptionFilter>();
        })
    .AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            opts.JsonSerializerOptions.PropertyNamingPolicy = KebabCaseNamingPolicy.Instance;
        });

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<PFM.API.Contracts.ImportFileRequestValidator>();

// Override the automatic 400 response
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors =
            from kvp in context.ModelState
            from error in kvp.Value!.Errors
            select new
            {
                tag = string.IsNullOrWhiteSpace(kvp.Key)
                        ? null
                        : KebabCaseNamingPolicy.Instance.ConvertName(kvp.Key),
                error = "validation-error",
                message = string.IsNullOrWhiteSpace(error.ErrorMessage)
                            ? "The request is not valid."
                            : error.ErrorMessage
            };

        return new ObjectResult(new { errors })
        {
            StatusCode = StatusCodes.Status400BadRequest
        };
    };
});

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Auto‐apply any pending EF Core migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Enable Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowFrontendDev");

// Map attribute‐routed controllers
app.MapControllers();

app.Run();
