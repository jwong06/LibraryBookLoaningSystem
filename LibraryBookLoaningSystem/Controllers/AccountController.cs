﻿using LibraryBookLoaningSystem.IdentityModels;
using LibraryBookLoaningSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace LibraryBookLoaningSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Users> signInManager;
        private readonly UserManager<Users> userManager;
        private readonly ILogger<Users> _logger;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public AccountController(SignInManager<Users> signInManager, UserManager<Users> userManager, ILogger<Users> logger)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this._logger = logger;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);

                if (result.Succeeded)
                {
                    logger.Info("{username} successfully logged in on {date}.(NLog)", model.Email, DateTime.Now);
                    _logger.LogInformation("{username} successfully logged in on {date}.", model.Email, DateTime.Now);
                    return RedirectToAction("Index", "Home");
                }
                if (result.IsLockedOut)
                {
                    var changePassLink = Url.Action(nameof(ChangePassword), "Account", new { }, Request.Scheme);
                    //var content = string.Format("Your account is locked out, to reset your password, please click this link: {0}", changePassLink);
                    //var message = new Message(new string[] { userModel.Email }, "Locked out account information", content, null);
                    //await _emailSender.SendEmailAsync(message);
                    ModelState.AddModelError("", "The account is locked out.");
                    logger.Warn("{username} is locked out.(NLog)", model.Email);
                    _logger.LogError("{username} is locked out.", model.Email);

                    return View();
                }
                else
                {
                    ModelState.AddModelError("", "Email or password is incorrect.");
                    logger.Warn("Invalid login attempt from {username} on {date}.(NLog)", model.Email, DateTime.Now);
                    _logger.LogError("Invalid login attempt from {username} on {date}.", model.Email, DateTime.Now);
                    return View(model);
                }
            }
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                Users users = new Users
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    UserName = model.Email
                };
                var result = await userManager.CreateAsync(users, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
            }
            return View(model);
        }

        public IActionResult VerifyEmail()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = userManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Something is wrong!");
                    return View(model);
                }
                else
                {
                    return RedirectToAction("ChangePassword", "Account", new { username = user.Result.UserName });
                }
            }
            return View(model);
        }

        public IActionResult ChangePassword(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("VerifyEmail", "Account");
            }
            return View(new ChangePasswordViewModel { Email = username });
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Email);
                if (user != null) 
                {
                    var result = await userManager.RemovePasswordAsync(user);
                    if(result.Succeeded)
                    {
                        result = await userManager.AddPasswordAsync(user, model.NewPassword);
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(model);
                    }
                } else
                {
                    ModelState.AddModelError("", "Email not found.");
                    return View(model);
                }
            } else
            {
                ModelState.AddModelError("", "Something went wrong.");
                return View(model);

            }
        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}
