using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using Utilities;

namespace Database.DbInitilizer
{
    public class DbInitilizer : IDbInitilizer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public DbInitilizer(RoleManager<IdentityRole> roleManager, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager= userManager;
            _context = context;
        }
        public void Init()
        {
            //migration
            try
            {
                if (_context.Database.GetPendingMigrations().Count() > 0)
                {
                    _context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {

            }


            //Roles
            if (!_roleManager.RoleExistsAsync(SD.Member).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Member)).GetAwaiter().GetResult();

                _userManager.CreateAsync(new ApplicationUser
                {
                    Name = SD.Admin,
                    isAuthorized = true,
                    UserName = "trackersuburban@gmail.com",
                    NormalizedUserName = "TRACKERSUBURBAN@GMAIL.COM",
                    Email = "trackersuburban@gmail.com",
                    NormalizedEmail = "TRACKERSUBURBAN@GMAIL.COM",
                    EmailConfirmed = true,
                }, "Admin@465").GetAwaiter().GetResult();


                ApplicationUser user = _context.applicationUsers.FirstOrDefault(u => u.Email == "trackersuburban@gmail.com");
                _userManager.AddToRoleAsync(user, SD.Admin).GetAwaiter().GetResult();
            }
            return;
           
        }
    }
}
