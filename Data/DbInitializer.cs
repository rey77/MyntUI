using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyntUI.Data
{
    public interface IDatabaseInitializer
    {
        Task Initialize();
    }

    public class ApplicationUser : IdentityUser
    {
    }

    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DatabaseInitializer(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task Initialize()
        {
            _context.Database.EnsureCreated();

            string userName = "admin";
            string userEmail = "admin";
            string userPassword = "admin";

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                var userCreated = new ApplicationUser { UserName = userName, Email = userEmail, EmailConfirmed = true };
                var resultCreateAsync = await _userManager.CreateAsync(userCreated, userPassword);
            }
            else
            {
                var resultDeletePassword = await _userManager.RemovePasswordAsync(user);
                var resultResetPassword = await _userManager.AddPasswordAsync(user, userPassword);
            }

        }
    }
}
