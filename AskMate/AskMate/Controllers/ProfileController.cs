using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}