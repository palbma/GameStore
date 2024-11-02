using Microsoft.AspNetCore.Identity;

internal class RoleInitializer
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roleNames = { "user", "admin", "superuser" };
        IdentityResult roleResult;

        foreach (var roleName in roleNames)
        {

            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();


        var adminUser = await userManager.FindByEmailAsync("admin@a.com");
        if (adminUser == null)
        {
            var admin = new IdentityUser
            {
                UserName = "admin@a.com",
                Email = "admin@a+" +
                ".com"
            };

            var createAdminResult = await userManager.CreateAsync(admin, "Admin@123");
            if (createAdminResult.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "admin");
            }
        }
    }
}