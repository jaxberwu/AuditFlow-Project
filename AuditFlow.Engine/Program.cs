using Microsoft.EntityFrameworkCore;
using AuditFlow.Shared.Data;
using AuditFlow.Shared.Entities;
using AuditFlow.Shared.DTOs;
using System.Net.Http.Json;

var builder = WebApplication.CreateBuilder(args);

// Configure port 5002
builder.WebHost.UseUrls("http://localhost:5002");

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

// Configure HttpClient to call CS_Simulator
var simulatorApiUrl = builder.Configuration["SimulatorApiUrl"] ?? "http://localhost:5001";
builder.Services.AddHttpClient("Simulator", client =>
{
    client.BaseAddress = new Uri(simulatorApiUrl);
});

// Configure EF Core with SQL Server - INDEPENDENT DATABASE
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=(localdb)\\mssqllocaldb;Database=AuditFlowDB;Trusted_Connection=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<AuditFlowDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuditFlowDbContext>();
    db.Database.EnsureCreated();
    
    // Seed hardware assets if empty (5-10 devices)
    // IMPORTANT: Use hostnames from Simulator to ensure matching
    if (!db.HardwareAssets.Any())
    {
        var random = new Random();
        var assets = new List<HardwareAsset>();
        
        // Try to get hostnames from Simulator API
        // Format: Hostname = "D" + SerialNumber (e.g., "DX35GB8")
        // SN field stores only the serial number part (e.g., "X35GB8")
        List<string> serialNumbers = new();
        try
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(simulatorApiUrl), Timeout = TimeSpan.FromSeconds(5) };
            var threatSummary = await httpClient.GetFromJsonAsync<ThreatSummaryDto>("/api/v1/threats/summary");
            if (threatSummary?.HostThreatCounts != null && threatSummary.HostThreatCounts.Any())
            {
                // Extract serial numbers from hostnames (remove "D" prefix)
                // Simulator has "DX35GB8", we store SN as "X35GB8" and Hostname as "DX35GB8"
                serialNumbers = threatSummary.HostThreatCounts
                    .Select(h => h.Hostname.StartsWith("D", StringComparison.OrdinalIgnoreCase) 
                        ? h.Hostname.Substring(1) 
                        : h.Hostname)
                    .Distinct()
                    .ToList();
            }
        }
        catch
        {
            // If Simulator is not available, generate random serial numbers
            // This will be matched later when Simulator is available
        }
        
        // If no serial numbers from Simulator, generate some (will be matched on next audit)
        if (!serialNumbers.Any())
        {
            string GenerateSerialNumber()
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var length = random.Next(5, 9);
                // Generate random serial number (without D prefix)
                return new string(Enumerable.Repeat(chars, length)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            }
            
            for (int i = 0; i < random.Next(5, 11); i++)
            {
                serialNumbers.Add(GenerateSerialNumber());
            }
        }
        
        // Use serial numbers from Simulator (or generated ones) to create assets
        var deviceCount = Math.Min(random.Next(5, 11), serialNumbers.Count);
        var selectedSerialNumbers = serialNumbers.Take(deviceCount).ToList();
        
        for (int i = 0; i < selectedSerialNumbers.Count; i++)
        {
            var serialNumber = selectedSerialNumbers[i];
            var hostname = "D" + serialNumber; // Hostname = D + SerialNumber
            
            // Mix: Some devices from 2018, some from 2024
            DateTime purchaseDate;
            if (i < selectedSerialNumbers.Count / 2) // First half are old devices (2018)
            {
                var month = random.Next(1, 13);
                var day = random.Next(1, DateTime.DaysInMonth(2018, month) + 1);
                purchaseDate = new DateTime(2018, month, day);
            }
            else // Second half are new devices (2024)
            {
                var month = random.Next(1, 13);
                var day = random.Next(1, DateTime.DaysInMonth(2024, month) + 1);
                purchaseDate = new DateTime(2024, month, day);
            }
            
            assets.Add(new HardwareAsset
            {
                SN = serialNumber, // SN stores the serial number part (e.g., "X35GB8")
                Hostname = hostname, // Hostname = D + SerialNumber (e.g., "DX35GB8")
                PurchaseDate = purchaseDate,
                Status = "Active"
            });
        }
        
        db.HardwareAssets.AddRange(assets);
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

// Minimal API endpoint: GET /api/audit/summary
// Add simple in-memory cache to reduce API calls
var threatSummaryCache = new System.Collections.Concurrent.ConcurrentDictionary<string, (ThreatSummaryDto data, DateTime expires)>();

app.MapGet("/api/audit/summary", async (AuditFlowDbContext db, IHttpClientFactory httpClientFactory) =>
{
    try
    {
        // Step 1: Fetch threat summary from CS_Simulator (with caching)
        ThreatSummaryDto? threatSummaryResponse = null;
        var cacheKey = "threat_summary";
        var now = DateTime.UtcNow;
        
        // Check cache (5 second cache)
        if (threatSummaryCache.TryGetValue(cacheKey, out var cached) && cached.expires > now)
        {
            threatSummaryResponse = cached.data;
        }
        else
        {
            var httpClient = httpClientFactory.CreateClient("Simulator");
            threatSummaryResponse = await httpClient.GetFromJsonAsync<ThreatSummaryDto>("/api/v1/threats/summary");
            
            // Cache for 5 seconds
            if (threatSummaryResponse != null)
            {
                threatSummaryCache[cacheKey] = (threatSummaryResponse, now.AddSeconds(5));
            }
        }
        
        if (threatSummaryResponse == null)
        {
            return Results.Problem("Failed to fetch threat data from Simulator");
        }
        
        // Step 2: Get all hardware assets from local database
        var assets = await db.HardwareAssets.ToListAsync();
        var currentYear = DateTime.Now.Year;
        
        // Step 3: Create a set of hostnames with threats for quick lookup
        // Format: Simulator hostnames = "D" + SerialNumber (e.g., "DX35GB8")
        // Engine stores: SN = SerialNumber (e.g., "X35GB8"), Hostname = "D" + SerialNumber (e.g., "DX35GB8")
        var hostnamesWithThreats = threatSummaryResponse.HostThreatCounts
            .Where(h => h.ThreatCount > 0)
            .Select(h => h.Hostname)
            .ToHashSet();
        
        // Step 4: Audit Logic (Q1-Q5)
        // Rule: Device in threat list AND age >= 5 years = Non-Compliant
        var complianceItems = assets.Select(asset =>
        {
            var ageYears = currentYear - asset.PurchaseDate.Year;
            // Asset.Hostname is already in format "D" + SerialNumber (e.g., "DX35GB8")
            // Simulator hostnames are also "D" + SerialNumber, so direct comparison works
            var hasThreat = hostnamesWithThreats.Contains(asset.Hostname);
            var threatCount = threatSummaryResponse.HostThreatCounts
                .FirstOrDefault(h => h.Hostname == asset.Hostname)?.ThreatCount ?? 0;
            
            // Compliance determination
            string status;
            string action;
            string reason;
            
            if (hasThreat && ageYears >= 5)
            {
                status = "Non-Compliant";
                action = "Replace";
                reason = $"Device is {ageYears} years old and has {threatCount} threat(s)";
            }
            else if (hasThreat)
            {
                status = "Compliant";
                action = "Monitor";
                reason = $"Device has {threatCount} threat(s) but age is {ageYears} years (< 5 years)";
            }
            else if (ageYears >= 5)
            {
                status = "Compliant";
                action = "Monitor";
                reason = $"Device is {ageYears} years old but no threats detected";
            }
            else
            {
                status = "Compliant";
                action = "Monitor";
                reason = "Device is compliant";
            }
            
            return new ComplianceItem
            {
                SN = asset.SN,
                Hostname = asset.Hostname,
                PurchaseDate = asset.PurchaseDate,
                AgeYears = ageYears,
                Status = status,
                Action = action,
                ThreatCount = threatCount,
                Reason = reason
            };
        }).ToList();
        
        var summary = new AuditSummaryDto
        {
            ComplianceItems = complianceItems,
            TotalAssets = assets.Count,
            CompliantCount = complianceItems.Count(c => c.Status == "Compliant"),
            NonCompliantCount = complianceItems.Count(c => c.Status == "Non-Compliant")
        };
        
        return Results.Ok(summary);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error during audit: {ex.Message}");
    }
})
.WithName("GetAuditSummary")
.WithOpenApi();

app.Run();
