using Microsoft.EntityFrameworkCore;
using AuditFlow.Shared.Data;
using AuditFlow.Shared.Entities;
using AuditFlow.Shared.DTOs;
using System.Net.Http.Json;

var builder = WebApplication.CreateBuilder(args);

// Configure port 5002 - 配置端口 5002
builder.WebHost.UseUrls("http://localhost:5002");

// Add services - 添加服务
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

// Configure HttpClient to call CS_Simulator - 配置 HttpClient 用于调用 CS_Simulator
var simulatorApiUrl = builder.Configuration["SimulatorApiUrl"] ?? "http://localhost:5001";
builder.Services.AddHttpClient("Simulator", client =>
{
    client.BaseAddress = new Uri(simulatorApiUrl);
});

// Configure EF Core with SQL Server - INDEPENDENT DATABASE - 配置 EF Core 使用 SQL Server - 独立数据库
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=(localdb)\\mssqllocaldb;Database=AuditFlowDB;Trusted_Connection=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<AuditFlowDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Ensure database is created and seeded - 确保数据库已创建并填充种子数据
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuditFlowDbContext>();
    db.Database.EnsureCreated();
    
    // Seed hardware assets if empty (5-10 devices) - 如果数据库为空，生成硬件资产种子数据（5-10 个设备）
    // IMPORTANT: Use hostnames from Simulator to ensure matching - 重要：使用 Simulator 的主机名以确保匹配
    if (!db.HardwareAssets.Any())
    {
        var random = new Random();
        var assets = new List<HardwareAsset>();
        
        // Try to get hostnames from Simulator API - 尝试从 Simulator API 获取主机名
        // Format: Hostname = "D" + SerialNumber (e.g., "DX35GB8") - 格式：Hostname = "D" + 序列号（例如："DX35GB8"）
        // SN field stores only the serial number part (e.g., "X35GB8") - SN 字段仅存储序列号部分（例如："X35GB8"）
        List<string> serialNumbers = new();
        try
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(simulatorApiUrl), Timeout = TimeSpan.FromSeconds(5) };
            var threatSummary = await httpClient.GetFromJsonAsync<ThreatSummaryDto>("/api/v1/threats/summary");
            if (threatSummary?.HostThreatCounts != null && threatSummary.HostThreatCounts.Any())
            {
                // Extract serial numbers from hostnames (remove "D" prefix) - 从主机名中提取序列号（移除 "D" 前缀）
                // Simulator has "DX35GB8", we store SN as "X35GB8" and Hostname as "DX35GB8" - Simulator 有 "DX35GB8"，我们存储 SN 为 "X35GB8"，Hostname 为 "DX35GB8"
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
            // If Simulator is not available, generate random serial numbers - 如果 Simulator 不可用，生成随机序列号
            // This will be matched later when Simulator is available - 这将在 Simulator 可用后匹配
        }
        
        // If no serial numbers from Simulator, generate some (will be matched on next audit) - 如果 Simulator 没有序列号，生成一些（将在下次审计时匹配）
        if (!serialNumbers.Any())
        {
            string GenerateSerialNumber()
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var length = random.Next(5, 9);
                // Generate random serial number (without D prefix) - 生成随机序列号（不带 D 前缀）
                return new string(Enumerable.Repeat(chars, length)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            }
            
            for (int i = 0; i < random.Next(5, 11); i++)
            {
                serialNumbers.Add(GenerateSerialNumber());
            }
        }
        
        // Use serial numbers from Simulator (or generated ones) to create assets - 使用来自 Simulator 的序列号（或生成的序列号）创建资产
        var deviceCount = Math.Min(random.Next(5, 11), serialNumbers.Count);
        var selectedSerialNumbers = serialNumbers.Take(deviceCount).ToList();
        
        for (int i = 0; i < selectedSerialNumbers.Count; i++)
        {
            var serialNumber = selectedSerialNumbers[i];
            var hostname = "D" + serialNumber; // Hostname = D + SerialNumber - 主机名 = D + 序列号
            
            // Mix: Some devices from 2018, some from 2024 - 混合：一些设备来自 2018 年，一些来自 2024 年
            DateTime purchaseDate;
            if (i < selectedSerialNumbers.Count / 2) // First half are old devices (2018) - 前半部分是旧设备（2018 年）
            {
                var month = random.Next(1, 13);
                var day = random.Next(1, DateTime.DaysInMonth(2018, month) + 1);
                purchaseDate = new DateTime(2018, month, day);
            }
            else // Second half are new devices (2024) - 后半部分是新设备（2024 年）
            {
                var month = random.Next(1, 13);
                var day = random.Next(1, DateTime.DaysInMonth(2024, month) + 1);
                purchaseDate = new DateTime(2024, month, day);
            }
            
            assets.Add(new HardwareAsset
            {
                SN = serialNumber, // SN stores the serial number part (e.g., "X35GB8") - SN 存储序列号部分（例如："X35GB8"）
                Hostname = hostname, // Hostname = D + SerialNumber (e.g., "DX35GB8") - 主机名 = D + 序列号（例如："DX35GB8"）
                PurchaseDate = purchaseDate,
                Status = "Active"
            });
        }
        
        db.HardwareAssets.AddRange(assets);
        db.SaveChanges();
    }
}

// Configure pipeline - 配置中间件管道
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.UseHttpsRedirection();

// Minimal API endpoint: GET /api/audit/summary - Minimal API 端点：GET /api/audit/summary
// Add simple in-memory cache to reduce API calls - 添加简单的内存缓存以减少 API 调用
var threatSummaryCache = new System.Collections.Concurrent.ConcurrentDictionary<string, (ThreatSummaryDto data, DateTime expires)>();

app.MapGet("/api/audit/summary", async (AuditFlowDbContext db, IHttpClientFactory httpClientFactory) =>
{
    try
    {
        // Step 1: Fetch threat summary from CS_Simulator (with caching) - 步骤 1：从 CS_Simulator 获取威胁摘要（带缓存）
        ThreatSummaryDto? threatSummaryResponse = null;
        var cacheKey = "threat_summary";
        var now = DateTime.UtcNow;
        
        // Check cache (5 second cache) - 检查缓存（5 秒缓存）
        if (threatSummaryCache.TryGetValue(cacheKey, out var cached) && cached.expires > now)
        {
            threatSummaryResponse = cached.data;
        }
        else
        {
            var httpClient = httpClientFactory.CreateClient("Simulator");
            threatSummaryResponse = await httpClient.GetFromJsonAsync<ThreatSummaryDto>("/api/v1/threats/summary");
            
            // Cache for 5 seconds - 缓存 5 秒
            if (threatSummaryResponse != null)
            {
                threatSummaryCache[cacheKey] = (threatSummaryResponse, now.AddSeconds(5));
            }
        }
        
        if (threatSummaryResponse == null)
        {
            return Results.Problem("Failed to fetch threat data from Simulator");
        }
        
        // Step 2: Get all hardware assets from local database - 步骤 2：从本地数据库获取所有硬件资产
        var assets = await db.HardwareAssets.ToListAsync();
        var currentYear = DateTime.Now.Year;
        
        // Step 3: Create a set of hostnames with threats for quick lookup - 步骤 3：创建带有威胁的主机名集合以便快速查找
        // Format: Simulator hostnames = "D" + SerialNumber (e.g., "DX35GB8") - 格式：Simulator 主机名 = "D" + 序列号（例如："DX35GB8"）
        // Engine stores: SN = SerialNumber (e.g., "X35GB8"), Hostname = "D" + SerialNumber (e.g., "DX35GB8") - Engine 存储：SN = 序列号（例如："X35GB8"），Hostname = "D" + 序列号（例如："DX35GB8"）
        var hostnamesWithThreats = threatSummaryResponse.HostThreatCounts
            .Where(h => h.ThreatCount > 0)
            .Select(h => h.Hostname)
            .ToHashSet();
        
        // Step 4: Audit Logic (Q1-Q5) - 步骤 4：审计逻辑（Q1-Q5）
        // Rule: Device in threat list AND age >= 5 years = Non-Compliant - 规则：设备在威胁列表中且年龄 >= 5 年 = 不合规
        var complianceItems = assets.Select(asset =>
        {
            var ageYears = currentYear - asset.PurchaseDate.Year;
            // Asset.Hostname is already in format "D" + SerialNumber (e.g., "DX35GB8") - Asset.Hostname 已经是 "D" + 序列号格式（例如："DX35GB8"）
            // Simulator hostnames are also "D" + SerialNumber, so direct comparison works - Simulator 主机名也是 "D" + 序列号，因此直接比较即可
            var hasThreat = hostnamesWithThreats.Contains(asset.Hostname);
            var threatCount = threatSummaryResponse.HostThreatCounts
                .FirstOrDefault(h => h.Hostname == asset.Hostname)?.ThreatCount ?? 0;
            
            // Compliance determination - 合规性判断
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
