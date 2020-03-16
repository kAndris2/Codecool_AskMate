using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AskMate.Models;

namespace AskMate.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult List()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Index(EditQuestionModel editedQuestion)
        {
            int Id = editedQuestion.Id;
            string Title = editedQuestion.Title;
            string Content = editedQuestion.Content;

            var edit = new IDAO_Impl();
            edit.EditLine(Id,Title,Content);
            
            return View();
        }

        [HttpGet("/question/{id}")]
        public IActionResult Question(int id)
        {
            foreach (QuestionModel item in new IDAO_Impl().GetQuestions())
            {
                if (id.Equals(item.Id))
                    return View("QuestionResponse",item);
            }
            return null;
        }
        
        [HttpGet("/edit/{id}")]
        public IActionResult Edit(int id)
        {
            foreach (QuestionModel item in new IDAO_Impl().GetQuestions())
            {
                if (id.Equals(item.Id))
                    return View("Edit", item);
            }
            return null;
        }

        [HttpGet("/details/{id}")]
        public IActionResult Details(int id)
        {
            foreach (QuestionModel item in new IDAO_Impl().GetQuestions())
            {
                if (id.Equals(item.Id))
                    return View("Details", item);
            }
            return null;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
