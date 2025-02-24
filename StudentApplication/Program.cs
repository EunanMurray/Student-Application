using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentApplicationPages.Data;
using StudentApplicationModel.Models;
using ScholarshipInfoSystem.Data;
using StudentApplicationModel.Data;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// **Add the following logging configuration:**
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Connection strings
var connectionString = builder.Configuration.GetConnectionString("Project300Database")
    ?? throw new InvalidOperationException("Connection string 'Project300Database' not found.");

// Configure DbContexts
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<PrimaryContext>(options =>
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("StudentApplication")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login"; // Redirect to login if not authenticated
    options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // Redirect if not authorized
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
});

builder.Services.AddRazorPages();

var app = builder.Build();

// Initialize Database and Roles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
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
            // Check if user is already in the specified role
            if (!await userManager.IsInRoleAsync(user, roleToAssign))
            {
                // Remove any existing roles the user may have
                var currentRoles = await userManager.GetRolesAsync(user);
                if (currentRoles.Any())
                {
                    await userManager.RemoveFromRolesAsync(user, currentRoles);
                    logger.LogInformation($"Removed existing roles from user {userEmail}: {string.Join(", ", currentRoles)}");
                }

                // Assign the new role
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
    }
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

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Lax,
    Secure = CookieSecurePolicy.Always
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapGet("/", async context =>
    {
        context.Response.Redirect("/Applications/Apply");
    });
});




app.Run();
