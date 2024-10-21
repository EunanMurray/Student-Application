using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentApplicationPages.Data;
using ScholarshipInfoSystem.Models;
using ScholarshipInfoSystem.Data;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Existing connection string for Identity
var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(defaultConnectionString));

// Register your PrimaryContext
var primaryConnectionString = builder.Configuration.GetConnectionString("PrimaryContext")
    ?? throw new InvalidOperationException("Connection string 'PrimaryContext' not found.");

builder.Services.AddDbContext<PrimaryContext>(options =>
    options.UseSqlServer(primaryConnectionString));

// Register your SecondaryContext (if necessary)
var secondaryConnectionString = builder.Configuration.GetConnectionString("SecondaryContext")
    ?? throw new InvalidOperationException("Connection string 'SecondaryContext' not found.");

builder.Services.AddDbContext<SecondaryContext>(options =>
    options.UseSqlServer(secondaryConnectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    // Map Razor Pages
    endpoints.MapRazorPages();

    // Redirect root URL "/" to "/Applications/Apply"
    endpoints.MapGet("/", async context =>
    {
        context.Response.Redirect("/Applications/Apply");
    });
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Ensure this is included if you're using Identity
app.UseAuthorization();

app.MapRazorPages();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<PrimaryContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        // Handle exceptions (e.g., log the error)
    }
}

app.Run();
