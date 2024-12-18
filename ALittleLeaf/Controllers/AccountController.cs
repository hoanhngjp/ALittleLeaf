﻿using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Index", "Login");
            }
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();

            ViewData["Cart"] = cart;

            return View();
        }
    }
}
