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
        private readonly UserManager<IdentityUser> _userManager;

        public DatabaseInitializer(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task Initialize()
        {
            _context.Database.EnsureCreated();

            string userName = "admin@localhost";
            string userEmail = "admin@localhost";
            string userPassword = "admin";

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                var userCreated = new IdentityUser { UserName = userName, Email = userEmail, EmailConfirmed = true };
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
