using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ChuvsuEvents.Models {
    public class RoleInitializer {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager) {
            string adminEmail = "chuvsuevents@yandex.ru";
            string password = "123456";
            if (await roleManager.FindByNameAsync("Администратор") == null) {
                await roleManager.CreateAsync(new IdentityRole("Администратор"));
            }
            if (await roleManager.FindByNameAsync("Студент") == null) {
                await roleManager.CreateAsync(new IdentityRole("Студент"));
            }
            if (await userManager.FindByNameAsync(adminEmail) == null) {
                User admin = new User { Email = adminEmail, UserName = adminEmail };
                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded) {
                    await userManager.AddToRoleAsync(admin, "Администратор");
                }
            }
        }
    }
}
