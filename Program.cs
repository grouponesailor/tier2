using Serilog;
using Tier2Management.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Tier 2 Management API", 
        Version = "v1",
        Description = "Administrative tools for corporate file management platform"
    });
});

// Add HttpClient for mock server communication
builder.Services.AddHttpClient<IFileManagementService, FileManagementService>();
builder.Services.AddHttpClient<IUserManagementService, UserManagementService>();
builder.Services.AddHttpClient<IQueueManagementService, QueueManagementService>();

// Add CORS for Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Register services
builder.Services.AddScoped<IFileManagementService, FileManagementService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IQueueManagementService, QueueManagementService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tier 2 Management API v1");
    });
}

app.UseSerilogRequestLogging();

app.UseCors("AllowAngularApp");

app.UseHttpsRedirection();

app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => new
{
    status = "Healthy",
    timestamp = DateTime.UtcNow,
    version = "1.0.0",
    environment = app.Environment.EnvironmentName
});

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Tier 2 Management API starting up");
logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);

app.Run();
