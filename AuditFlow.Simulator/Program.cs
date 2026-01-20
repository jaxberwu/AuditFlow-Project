using Microsoft.EntityFrameworkCore;
using AuditFlow.Shared.Data;
using AuditFlow.Shared.Entities;
using AuditFlow.Shared.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Configure port 5001
builder.WebHost.UseUrls("http://localhost:5001");

// Add services
builder.Services.AddOpenApi();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configure EF Core with SQL Server - INDEPENDENT DATABASE
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=(localdb)\\mssqllocaldb;Database=CS_SimulatorDB;Trusted_Connection=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<CS_SimulatorDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CS_SimulatorDbContext>();
    db.Database.EnsureCreated();
    
    // Seed random CVE threat data if empty (50-100 records)
    if (!db.CVEThreats.Any())
    {
        var random = new Random();
        var threats = new List<CVEThreat>();
        var cvePrefixes = new[] { "CVE-2023-", "CVE-2024-", "CVE-2022-", "CVE-2021-" };
        var severities = new[] { "Critical", "High", "Medium", "Low" };
        var remediations = new[]
        {
            "Apply security patch immediately",
            "Update to latest version",
            "Disable affected feature",
            "Apply workaround from vendor",
            "Upgrade system components",
            "Review and update access controls"
        };
        
        // Generate 50-100 random threats
        var threatCount = random.Next(50, 101);
        var hostnames = new List<string>();
        
        // Generate some hostnames (D + 5-8 alphanumeric)
        string GenerateHostname()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var length = random.Next(5, 9);
            return "D" + new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        
        // Generate 10-15 unique hostnames
        for (int i = 0; i < random.Next(10, 16); i++)
        {
            hostnames.Add(GenerateHostname());
        }
        
        // Generate threats for these hostnames
        for (int i = 0; i < threatCount; i++)
        {
            var hostname = hostnames[random.Next(hostnames.Count)];
            var cveId = cvePrefixes[random.Next(cvePrefixes.Length)] + random.Next(1000, 99999);
            var severity = severities[random.Next(severities.Length)];
            var remediation = remediations[random.Next(remediations.Length)];
            var detectedDate = DateTime.Now.AddDays(-random.Next(1, 365));
            
            threats.Add(new CVEThreat
            {
                Hostname = hostname,
                CVE_ID = cveId,
                Severity = severity,
                Remediation = remediation,
                DetectedDate = detectedDate
            });
        }
        
        db.CVEThreats.AddRange(threats);
        db.SaveChanges();
    }
}

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.UseHttpsRedirection();

// Minimal API endpoints
// GET /api/v1/threats/summary
app.MapGet("/api/v1/threats/summary", async (CS_SimulatorDbContext db) =>
{
    var threats = await db.CVEThreats.ToListAsync();
    
    var hostThreatCounts = threats
        .GroupBy(t => t.Hostname)
        .Select(g => new HostThreatCount
        {
            Hostname = g.Key,
            ThreatCount = g.Count(),
            CriticalCount = g.Count(t => t.Severity == "Critical"),
            HighCount = g.Count(t => t.Severity == "High"),
            MediumCount = g.Count(t => t.Severity == "Medium"),
            LowCount = g.Count(t => t.Severity == "Low")
        })
        .ToList();
    
    var summary = new ThreatSummaryDto
    {
        HostThreatCounts = hostThreatCounts,
        TotalThreats = threats.Count,
        TotalHosts = hostThreatCounts.Count
    };
    
    return Results.Ok(summary);
})
.WithName("GetThreatSummary")
.WithOpenApi();

// GET /api/v1/threats/details/{hostname}
app.MapGet("/api/v1/threats/details/{hostname}", async (CS_SimulatorDbContext db, string hostname) =>
{
    var threats = await db.CVEThreats
        .Where(t => t.Hostname == hostname)
        .ToListAsync();
    
    var details = new ThreatDetailsDto
    {
        Hostname = hostname,
        CVEs = threats.Select(t => new CVEThreatDetail
        {
            CVE_ID = t.CVE_ID,
            Severity = t.Severity,
            Remediation = t.Remediation,
            DetectedDate = t.DetectedDate
        }).ToList(),
        TotalCount = threats.Count
    };
    
    return Results.Ok(details);
})
.WithName("GetThreatDetails")
.WithOpenApi();

app.Run();
