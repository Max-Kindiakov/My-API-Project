﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ParkyWebSite;
using ParkyWebSite.Models;
using ParkyWebSite.Models.ViewModel;
using ParkyWebSite.Repository.IRepository;

namespace ParkyWebSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INationalParkRepository _npRepo;
        private readonly IAccountRepository _accRepo;
        private readonly ITrailRepository _trailRepo;
        public HomeController(ILogger<HomeController> logger, INationalParkRepository npRepo,
            ITrailRepository trailRepo, IAccountRepository accRepo)
        {
            _npRepo = npRepo;
            _trailRepo = trailRepo;
            _logger = logger;
            _accRepo = accRepo;
        }

        public async Task<IActionResult> Index()
        {
            IndexVM listOfParksAndTrails = new IndexVM()
            {
                NationalParkList = await _npRepo.GetAllAsync(Details.NationalParkApiPath,HttpContext.Session.GetString("JWToken")),
                TrailList = await _trailRepo.GetAllAsync(Details.TrailApiPath, HttpContext.Session.GetString("JWToken")),
            };
            return View(listOfParksAndTrails);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet]
        public IActionResult Login()
        {
            User obj = new User();
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //протидія підробці міжсайтових запитів
        public async Task<IActionResult> Login(User obj)
        {
            User objUser = await _accRepo.LoginAsync(Details.AccountApiPath + "authenticate/", obj);
            if (objUser.Token == null)
            {
                return View();
            }

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, objUser.Username));
            identity.AddClaim(new Claim(ClaimTypes.Role, objUser.Role));
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal); //логінимось


            HttpContext.Session.SetString("JWToken", objUser.Token);
            TempData["alert"] = "Вітаю вас " + objUser.Username+ "!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User obj)
        {
            bool result = await _accRepo.RegisterAsync(Details.AccountApiPath + "register/", obj);
            if (result == false)
            {
                return View();
            }
            TempData["alert"] = "Регістрація успішна";
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.SetString("JWToken", "");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        
        

    }
}
