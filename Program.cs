using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentApplicationPages.Data;
using ScholarshipInfoSystem.Models;
using ScholarshipInfoSystem.Data;

var builder = WebApplication.CreateBuilder(args);

// Connection strings
var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(defaultConnectionString));

var primaryConnectionString = builder.Configuration.GetConnectionString("PrimaryContext")
    ?? throw new InvalidOperationException("Connection string 'PrimaryContext' not found.");
builder.Services.AddDbContext<PrimaryContext>(options => options.UseSqlServer(primaryConnectionString));

var secondaryConnectionString = builder.Configuration.GetConnectionString("SecondaryContext")
    ?? throw new InvalidOperationException("Connection string 'SecondaryContext' not found.");
builder.Services.AddDbContext<SecondaryContext>(options => options.UseSqlServer(secondaryConnectionString));

builder.Services.AddDbContext<SecondaryContext>(options =>
    options.UseSqlServer(secondaryConnectionString), ServiceLifetime.Scoped);


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false) // Disabled account confirmation
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddRazorPages();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        //Following Chunk is for manually giving users roles via code.

        /*// Initialize roles
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        string[] roleNames = { "Admin", "Committee Member", "Viewer" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        string userEmail = "S00235207@atu.ie"; // Accounts email to be targeted
        string roleToAssign = "Admin"; // Accounts role you wanna give it

        var user = await userManager.FindByEmailAsync(userEmail);
        if (user != null)
        {
            if (!await userManager.IsInRoleAsync(user, roleToAssign))
            {
                var result = await userManager.AddToRoleAsync(user, roleToAssign);
                if (result.Succeeded)
                {
                    Console.WriteLine($"User with email {userEmail} has been assigned the role '{roleToAssign}'.");
                }
                else
                {
                    Console.WriteLine($"Failed to assign role '{roleToAssign}' to user with email {userEmail}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                Console.WriteLine($"User with email {userEmail} already has the role '{roleToAssign}'.");
            }
        }
        else
        {
            Console.WriteLine($"User with email {userEmail} not found.");
        }
        */

        // Initialize database
        var context = services.GetRequiredService<PrimaryContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        Console.WriteLine("An error occurred during role creation or DB initialization: " + ex.Message);
    }
}

if (app.Environment.IsDevelopment())
{
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
        context.Response.Redirect("/Applications/Apply");
    });
});

app.Run();
