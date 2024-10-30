using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentApplicationPages.Data;
using ScholarshipInfoSystem.Models;
using ScholarshipInfoSystem.Data;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(defaultConnectionString));

var primaryConnectionString = builder.Configuration.GetConnectionString("PrimaryContext")
    ?? throw new InvalidOperationException("Connection string 'PrimaryContext' not found.");

builder.Services.AddDbContext<PrimaryContext>(options =>
    options.UseSqlServer(primaryConnectionString));

var secondaryConnectionString = builder.Configuration.GetConnectionString("SecondaryContext")
    ?? throw new InvalidOperationException("Connection string 'SecondaryContext' not found.");

builder.Services.AddDbContext<SecondaryContext>(options =>
    options.UseSqlServer(secondaryConnectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddSingleton<IEmailSender, ConsoleEmailSender>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddRazorPages();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        // Add the admin role
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

       
        string roleName = "Admin";
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        string roleName2 = "Committe Member";
        if (!await roleManager.RoleExistsAsync(roleName2))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName2));
        }

        string roleName3 = "Viewer";
        if (!await roleManager.RoleExistsAsync(roleName3))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName3));
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("An error occurred while creating roles or assigning roles to users: " + ex.Message);

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

    }
}

app.Run();
