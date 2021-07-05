using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CinemaTickets.Domain.DomainModels;
using CinemaTickets.Domain.DomainModels.Enumerations;
using CinemaTickets.Domain.Identity;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CinemaTickets.Web.Controllers
{
    public class UserController : Controller
    {

        private readonly UserManager<CinemaApplicationUser> _userManager;
        private readonly Repository.ApplicationDbContext _context;

        public UserController(UserManager<CinemaApplicationUser> userManager,
             Repository.ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ManageRoles()
        {
            var users = _context.Users;
            return View(users);
        }


        public IActionResult ChangeRole(string userId)
        {
            var user = _context.Users.Find(userId);
            if(user.Role == Role.ADMIN)
            {
                user.Role = Role.USER;
            }
            else
            {
                user.Role = Role.ADMIN;
            }
            _context.SaveChanges();
            return RedirectToAction("ManageRoles", "User");
        }

        public IActionResult ImportUsers(IFormFile file)
        {
            string pathToUpload = $"{Directory.GetCurrentDirectory()}\\files\\{file.FileName}";
            using (FileStream fileStream = System.IO.File.Create(pathToUpload))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
            }
            List<UserRegistrationDto> users = getAllUsersFromFile(file.FileName);
            ImportAllUsers(users);
            return RedirectToAction("Index", "Order");
        }
        public bool ImportAllUsers(List<UserRegistrationDto> model)
        {
            bool status = true;

            foreach (var item in model)
            {
                var userCheck = _userManager.FindByEmailAsync(item.Email).Result;

                if (userCheck == null)
                {
                    var user = new CinemaApplicationUser
                    {
                        UserName = item.Email,
                        NormalizedUserName = item.Email,
                        Email = item.Email,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        ShoppingCart = new ShoppingCart(),
                        Role = item.Role
                    };
                    var result = _userManager.CreateAsync(user, item.Password).Result;

                    status = status && result.Succeeded;
                }
                else
                {
                    continue;
                }
            }

            return status;
        }

        private List<UserRegistrationDto> getAllUsersFromFile(string fileName)
        {

            List<UserRegistrationDto> users = new List<UserRegistrationDto>();

            string filePath = $"{Directory.GetCurrentDirectory()}\\files\\{fileName}";

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);


            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        users.Add(new UserRegistrationDto
                        {
                            Email = reader.GetValue(0).ToString(),
                            Password = reader.GetValue(1).ToString(),
                            ConfirmPassword = reader.GetValue(2).ToString(),
                            Role = (Role)Enum.Parse(typeof(Role), reader.GetValue(3).ToString())
                        });
                    }
                }
            }


            return users;
        }
    }
}
