using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AskMate.Models;
using AskMate;

namespace AskMate.Controllers
{
    public class QuestionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(EditQuestionModel newQuestion)
        {

            string Title = newQuestion.Title;
            string Content = newQuestion.Content;

            var newQ = IDAO_Impl.Instance;
            newQ.NewQuestion(Title, Content);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Question()
        {
            return View();
        }
    }
}