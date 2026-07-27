using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using AspDotnetBoilerplate.src.Domain;
using AspDotnetBoilerplate.src.Infrastructure;
using AspDotnetBoilerplate.src.Shared.Exceptions;
using AspDotnetBoilerplate.src.Shared.Exceptions.Handlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


string corsPolicySpecifiedOrigins = "_boilerplateAPIOriginPolicy";
var postgresConnectionString = builder.Configuration.GetConnectionString("Postgres")!;
var sqlServerConnectionString = builder.Configuration.GetConnectionString("SQLServer")!;
var redisConnectionString = builder.Configuration.GetConnectionString("Redis")!;


builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();  

builder.Services.AddControllers()
        .AddJsonOptions(options => {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
});

builder.Services.AddHybridCache(options =>
{
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(30),
        LocalCacheExpiration = TimeSpan.FromMinutes(5)
    };

    options.MaximumPayloadBytes = 1024 * 1024; 
});

// builder.Services.AddDbContext<IdentityAppDbContext>(options =>
//     options.UseNpgsql(postgresConnectionString));

// builder.Services.AddDbContext<IdentityAppDbContext>(options =>
//     options.UseSqlServer(postgresConnectionString));


builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));


var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };

    options.Events = new JwtBearerEvents
    {
      OnMessageReceived = context =>
      {
          var accessToken = context.Request.Cookies["access_token"];
           if (!string.IsNullOrEmpty(accessToken))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
      }
        
    };
});

builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<IdentityAppDbContext>()
.AddDefaultTokenProviders();


builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));

builder.Services.AddRateLimiter(cfg => cfg
    .AddFixedWindowLimiter(policyName: "fixed", opt =>
    {
        opt.PermitLimit = 4;
        opt.Window = TimeSpan.FromSeconds(12);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2;
    }));

builder.Services.AddRateLimiter(cfg =>
{
    cfg.AddSlidingWindowLimiter(policyName: "sliding", opt =>
    {
        opt.PermitLimit = 4;
        opt.Window = TimeSpan.FromSeconds(12);
        opt.SegmentsPerWindow = 4;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 4;
    });

    cfg.OnRejected = async (context, CancellationToken) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            
        }
    };
});

builder.Services.AddRateLimiter(cfg => cfg
    .AddTokenBucketLimiter(policyName: "token", opt =>
    {
        opt.AutoReplenishment = true;
        opt.QueueLimit = 4;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.ReplenishmentPeriod = TimeSpan.FromSeconds(20);
        opt.TokenLimit = 20;
        opt.TokensPerPeriod = 5;
    }));

builder.Services.AddRateLimiter(cfg => cfg
    .AddConcurrencyLimiter(policyName: "concurrency", opt =>
    {
        opt.PermitLimit = 4;
        opt.QueueLimit = 4;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;

    }));

builder.Services.AddProblemDetails(config =>
{
    config.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
    };
});

builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddHealthChecks();



List<string> origins = [
    "http://localhost:3000"
];

builder.Services.AddCors(options =>
{
   options.AddPolicy(corsPolicySpecifiedOrigins, policy =>
   {
        policy.WithOrigins([..origins])
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
   });
});


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My API v1");
        options.RoutePrefix = "swagger";
    });

}

app.UseRateLimiter();

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(corsPolicySpecifiedOrigins);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

