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
        public IDAO_Impl Idao = new IDAO_Impl();
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

            Idao.EditLine(Id,Title,Content);
            
            return View();
        }


        /*
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
        */

        //Modal
        [HttpGet("/Modal/{id}")]
        public ActionResult Modal(int id)
        {
            foreach (QuestionModel item in new IDAO_Impl().GetQuestions())
            {
                if (id.Equals(item.Id))
                    return View("Index", item);
            }
            return null;
        }


        [HttpGet("/edit/{id}")]
        public IActionResult Edit(int id)
        {
            foreach (QuestionModel item in Idao.GetQuestions())
            {
                if (id.Equals(item.Id))
                    return View("Edit", item);
            }
            return null;
        }

        [HttpGet("/question/{id}")]
        public IActionResult Question(int id)
        {
            foreach (QuestionModel item in Idao.GetQuestions())
            {
                if (id.Equals(item.Id))
                    return View("Question", item);
            }
            return null;
        }

        [HttpGet("/delete/{id}")]
        public IActionResult Delete(int id)
        {
            Idao.DeleteQuestion(id);
            return View("Index");
        }

       // [HttpPost("NewAnswer{id}")]
        public ActionResult NewAnswer([FromForm(Name = "answer")] string answer, int id)
        {
            /*
            foreach (QuestionModel item in Idao.GetQuestions())
            {
                if (id.Equals(item.Id))
                    return View("NewAnswer", item);
            }
            */
            return View("NewAnswer", new AnswerModel(69, answer));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
