using Microsoft.AspNetCore.Identity;

namespace CarQuery.Services
{
    public class SeedUserRoleInitial : ISeedUserRoleInitial
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedUserRoleInitial(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void SeedRoles()
        {
            if (!_roleManager.RoleExistsAsync("SuperAdmin").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "SuperAdmin";
                role.NormalizedName = "SUPERADMIN";
                IdentityResult roleResult = _roleManager.CreateAsync(role).Result;
            }
        }

        public void SeedUsers()
        {
            if (_userManager.FindByEmailAsync("CQ_superAdmin@gmail.com").Result == null)
            {
                IdentityUser user = new IdentityUser();

                user.UserName = "Super Admin";
                user.Email = "CQ_superAdmin@gmail.com";
                user.NormalizedUserName = "SUPER ADMIN";
                user.NormalizedEmail = "CQ_SUPERADMIN@GMAIL.COM";
                user.EmailConfirmed = true;
                user.LockoutEnabled = false;
                user.SecurityStamp = Guid.NewGuid().ToString();

                IdentityResult result = _userManager.CreateAsync(user, "SuperAdminPassword").Result;

                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, "SuperAdmin").Wait();
                }
                else
                {
                    Console.WriteLine("Mensagem de erro: ");

                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Erro: {error.Description}");
                    }
                }
            }
        }
    }
}
