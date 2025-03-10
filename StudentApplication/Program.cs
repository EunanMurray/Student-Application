using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentApplicationPages.Data;
using StudentApplicationModel.Models;
using ScholarshipInfoSystem.Data;
using StudentApplicationModel.Data;
using Microsoft.Extensions.Logging;
using StudentApplication.Services;
using StudentApplication.Settings;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

var cultureInfo = (CultureInfo)CultureInfo.GetCultureInfo("fr-FR").Clone();

cultureInfo.NumberFormat.CurrencySymbol = "€";
cultureInfo.NumberFormat.CurrencyPositivePattern = 0;
cultureInfo.NumberFormat.CurrencyNegativePattern = 0;


CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
builder.Logging.ClearProviders();

// Add logging configuration
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Connection strings
var connectionString = builder.Configuration.GetConnectionString("Project300Database")
    ?? throw new InvalidOperationException("Connection string 'Project300Database' not found.");

// Configure services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<PrimaryContext>(options =>
    options.UseSqlServer(connectionString, b =>
    {
        b.MigrationsAssembly("StudentApplication");
        b.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    }));

// Configure Email Service
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
});

builder.Services.AddRazorPages();

var app = builder.Build();

// Initialize Database and Roles
async Task InitializeDatabaseAsync(IServiceProvider services)
{
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var primaryContext = services.GetRequiredService<PrimaryContext>();
        var applicationContext = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Initialize both contexts and roles
        DbInitializer.Initialize(primaryContext, applicationContext, userManager, roleManager);

        logger.LogInformation("Database initialization completed successfully.");

        // Manually Assign Role to User
        string userEmail = "eunanmurray56@gmail.com";
        string roleToAssign = "Admin";

        var user = await userManager.FindByEmailAsync(userEmail);
        if (user != null)
        {
            if (!await userManager.IsInRoleAsync(user, roleToAssign))
            {
                var currentRoles = await userManager.GetRolesAsync(user);
                if (currentRoles.Any())
                {
                    await userManager.RemoveFromRolesAsync(user, currentRoles);
                    logger.LogInformation($"Removed existing roles from user {userEmail}: {string.Join(", ", currentRoles)}");
                }

                var result = await userManager.AddToRoleAsync(user, roleToAssign);
                if (result.Succeeded)
                {
                    logger.LogInformation($"User {userEmail} assigned role: {roleToAssign}");
                }
                else
                {
                    logger.LogWarning($"Failed to assign role {roleToAssign} to {userEmail}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                logger.LogInformation($"User {userEmail} is already in role {roleToAssign}");
            }
        }
        else
        {
            logger.LogWarning($"User {userEmail} not found.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database initialization");
        throw;
    }
}

// Execute database initialization
using (var scope = app.Services.CreateScope())
{
    await InitializeDatabaseAsync(scope.ServiceProvider);
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapGet("/", async context =>
    {
        context.Response.Redirect("/Applications/RegisterOrLogin");
    });
});

// Configure cookie policy
app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Lax,
    Secure = CookieSecurePolicy.Always
});

app.Run();