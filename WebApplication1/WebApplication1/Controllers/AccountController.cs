using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Helpers;
using WebApplication1.Models;
using WebApplication1.ViewModels.Account;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUser> _userManager { get; }
        private SignInManager<AppUser> _signInManager { get; }
        private RoleManager<IdentityRole> _roleManager { get; }

        public AccountController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }
                AppUser newUser = new AppUser
            {
                FullName = user.FullName,
                UserName=user.Username,
                Email = user.Email
            };
            var IdentityResult = await _userManager.CreateAsync(newUser, user.Password);
            if (!IdentityResult.Succeeded)
            {
                foreach (var error in IdentityResult.Errors)
                {
                    ModelState.AddModelError("" , error.Description);
                }
                return View(user);
            }
            await _userManager.AddToRoleAsync(newUser, Role.RoleType.Member.ToString());
            await _signInManager.SignInAsync(newUser, true);
            return RedirectToAction("Index","Home");
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");

        }
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async  Task<IActionResult> SignIn(SignInVM user)
        {
          AppUser userDb=await _userManager.FindByEmailAsync(user.Email);
            if (userDb == null)
            {
                ModelState.AddModelError("","Email or password incorrect!");
                return View(user);
            }
            
            var signInResult = await _signInManager.PasswordSignInAsync(userDb.UserName, user.Password, user.IsPersistent, lockoutOnFailure: true);
            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError("","Please try a few minutes later");
                return View(user);
            }
            if (!signInResult.Succeeded)
            {
                 ModelState.AddModelError("","Email or password is wrong");
                return View(user);
            }
            if (userDb.IsActivated)
            {
                 ModelState.AddModelError("","Please verify your account");
                return View(user);
            }
            return RedirectToAction("Index", "Home");

        }
        //public async Task CreateRole()
        //{
        //    foreach (var role in Enum.GetValues(typeof(Role.RoleType)))
        //    {
        //        if (!await _roleManager.RoleExistsAsync(role.ToString()))
        //        {
        //            await _roleManager.CreateAsync(new IdentityRole { Name = role.ToString() });
        //        }

        //    }
        //}

    }
}
