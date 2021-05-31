using ExampleService.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleService.Infrastructure
{
    public static class MigrationManager
    {
        public static async Task<IHost> MigrateDatabase(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                using (var appContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                {
                    try
                    {
                        appContext.Database.Migrate();
                        var userManager = (UserManager<User>)scope.ServiceProvider.GetService(typeof(UserManager<User>));
                        var user = await userManager.FindByNameAsync("admin");
                        if (user == null)
                        {
                            user = new User("admin", true);
                            var createResult = await userManager.CreateAsync(user);
                            if (createResult.Succeeded)
                            {
                                var addPassResult = await userManager.AddPasswordAsync(user, "Admin@123");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //Log errors or do anything you think it's needed
                        throw;
                    }
                }
            }
            return host;
        }
    }
}
