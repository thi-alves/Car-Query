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
            if (!_roleManager.RoleExistsAsync("Admin").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Admin";
                role.NormalizedName = "ADMIN";
                IdentityResult roleResult = _roleManager.CreateAsync(role).Result;
            }
        }

        public void SeedUsers()
        {
            if (_userManager.FindByEmailAsync("thiago.stdy@gmail.com").Result == null)
            {
                IdentityUser user = new IdentityUser();

                user.UserName = "Thiago Alves";
                user.Email = "thiago.stdy@gmail.com";
                user.NormalizedUserName = "THIAGO ALVES";
                user.NormalizedEmail = "THIAGO.STDY@GMAIL.COM";
                user.EmailConfirmed = true;
                user.LockoutEnabled = false;
                user.SecurityStamp = Guid.NewGuid().ToString();

                IdentityResult result = _userManager.CreateAsync(user, "AdminPassword").Result;

                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, "Admin").Wait();
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
