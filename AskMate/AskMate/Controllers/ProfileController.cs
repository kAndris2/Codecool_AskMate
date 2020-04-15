using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AskMate.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using AskMate.Models;

namespace AskMate.Controllers
{
    public class ProfileController : Controller
    {
        public IDAO_Impl Idao = IDAO_Impl.Instance;

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet("/user/{id}")]
        public IActionResult User(int id)

        {
            return View("UserPage", Idao.GetUserById(id));
        }
        //logintest
        


    }
}