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
    public class HomeController : Controller
    {
        public IDAO_Impl Idao;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            Idao = IDAO_Impl.Instance;
        }

        public IActionResult Index()
        {
            return View(Idao);
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

            Idao.EditLine(Id, Title, Content);

            return View();
        }

       
        public ActionResult Moddel([FromForm(Name = "id")] int delid)
        {
            Idao.DeleteQuestion(delid);
            return View("Index");
        }
        public IActionResult ModalId(int id)
        {
            foreach (QuestionModel item in Idao.GetQuestions())
            {
                if (id.Equals(item.Id))
                  return PartialView("_ModalPartialView", item);
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

        public ActionResult NewAnswer([FromForm(Name = "answer")] string answer, [FromForm(Name = "id")] int id)
        {
            foreach (QuestionModel question in Idao.GetQuestions())
            {
                if (id.Equals(question.Id))
                {
                    question.AddAnswer(new AnswerModel(question.Answers.Count+1, answer, DateTimeOffset.Now.ToUnixTimeMilliseconds(), 0));
                    return View("Question", question);
                }
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
