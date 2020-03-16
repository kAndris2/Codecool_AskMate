using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AskMate.Models;

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
            
            string Title =  newQuestion.Title;
            string Content = newQuestion.Content;

            var newQ = new IDAO_Impl();
            newQ.NewQuestion(Title,Content);

            return View();
        }
    }
}