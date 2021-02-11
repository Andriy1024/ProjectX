using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectX.Core;
using ProjectX.Core.Auth;
using ProjectX.Identity.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Identity.Persistence.Startup
{
    public class AspNetIdentityStartupTask : IStartupTask
    {
        readonly IdentityDbContext _dbContext;
        readonly RoleManager<RoleEntity> _roleManager;
        readonly UserManager<UserEntity> _userManager;

        public AspNetIdentityStartupTask(IdentityDbContext dbContext, 
               RoleManager<RoleEntity> roleManager, 
               UserManager<UserEntity> userManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            _dbContext.Database.Migrate();

            await SeedRoleAsync(IdentityRoles.User);
            await SeedRoleAsync(IdentityRoles.Admin);
            await SeedAdminAsync();
        }

        private async Task SeedRoleAsync(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var result = await _roleManager.CreateAsync(new RoleEntity(roleName));

                if (!result.Succeeded)
                    throw new Exception($"Error on adding {roleName} role.");
            }
        }

        private async Task SeedAdminAsync()
        {
            if (await _userManager.Users.AllAsync(us => us.Email != "admin@platform.com"))
            {
                var admin = UserEntity.Factory
                                      .Email("admin@projectX.com")
                                      .Name("Admin", "Admin")
                                      .Address(new Address("USA", "Redmond", "Building 92"))
                                      .Build();

                admin.EmailConfirmed = true;
                admin.PhoneNumberConfirmed = true;
                admin.PhoneNumber = "+10000000000";

                await _userManager.CreateAsync(admin, "AdminPass123$$");
                await _userManager.AddToRoleAsync(admin, IdentityRoles.Admin);
            }
        }
    }
}
