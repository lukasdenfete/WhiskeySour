using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using WhiskeySour.DataLayer;
using Microsoft.EntityFrameworkCore;
//using WhiskeySour.BusinessLayer.Services;
using WhiskeySour.DataLayer;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    await CreateRoles(roleManager, userManager);
}
app.UseStaticFiles();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();
app.Run();

async Task CreateRoles(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
{
    string[] roleNames = { "Admin", "User" };

    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            var role = new IdentityRole(roleName);
            await roleManager.CreateAsync(role);
        }
    }
    var adminUser = await userManager.FindByEmailAsync("lukas.rosendahl@hotmail.com");
    if (adminUser == null)
    {
        var user = new User { UserName = "lukas.rosendahl@hotmail.com", Email = "lukas.rosendahl@hotmail.com", FirstName = "Lukas", LastName = "Lukrecio"};
        var createAdminResult = await userManager.CreateAsync(user, "Admin123!");
        if (createAdminResult.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
    else
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");

    }
}