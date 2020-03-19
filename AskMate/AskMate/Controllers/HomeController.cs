using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AskMate.Models;
using AskMate;
using System.IO;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace AskMate.Controllers
{
    public class HomeController : Controller
    {
        public IDAO_Impl Idao;
        private readonly ILogger<HomeController> _logger;
        
        IHostingEnvironment _env;

        public HomeController(ILogger<HomeController> logger, IHostingEnvironment environment)
        {
            _logger = logger;
            Idao = IDAO_Impl.Instance;
            _env = environment;
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

            return View(Idao);
        }

       
        public ActionResult Moddel([FromForm(Name = "id")] int delid)
        {
            Idao.DeleteQuestion(delid);
            return View("Index", Idao);
        }

        public IActionResult ModalId(int id)
        {
            return PartialView("_ModalPartialView", Idao.GetQuestionById(id));
        }

        //Image
        [HttpPost]
        public async Task<IActionResult> ImageUpload(IFormFile file, int id)
        {
            if(file != null && file.Length > 0)
            {
                var imagePath = @"\Upload\Images\";
                var uploadPath = _env.WebRootPath + imagePath;

                //create dir
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                //uniq file name
                var uniqFileName = Guid.NewGuid().ToString();
                var filename = Path.GetFileName(uniqFileName + "." + file.FileName.Split(".")[1].ToLower());
                string fullPath = uploadPath + filename;

                imagePath = imagePath + @"\";
                var filePath = @".." + Path.Combine(imagePath, filename);

                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                ViewData["FileLocation"] = filePath;
                Idao.AddLinkToQuestion(filePath, id);
            }
            return View("Question", Idao.GetQuestionById(id));
        }

        [HttpGet("/delete/{id}")]
        public IActionResult Delete(long id)
        {
            QuestionModel instance = null;
            foreach (QuestionModel question in Idao.GetQuestions())
            {
                foreach (AnswerModel answer in question.Answers)
                {
                    if (id.Equals(answer.GetUnique()))
                    {
                        question.DeleteAnswer(answer);
                        instance = question;
                        break;
                    }
                }
            }
            return View("Question", instance);
        }

        [HttpGet("/edit/{id}")]
        public IActionResult Edit(int id)
        {
            return View("Edit", Idao.GetQuestionById(id));
        }

        [HttpGet("/question/{id}")]
        public IActionResult Question(int id)
        {
            return View("Question", Idao.GetQuestionById(id));
        }

        public ActionResult NewAnswer([FromForm(Name = "answer")] string answer, [FromForm(Name = "id")] int id)
        {
            QuestionModel question = Idao.GetQuestionById(id);
            question.AddAnswer(new AnswerModel(question.Answers.Count + 1, answer, DateTimeOffset.Now.ToUnixTimeMilliseconds(), 0, question.Id));
            return View("Question", question);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult UpVote([FromRoute(Name = "id")]int id)
        {
            QuestionModel question = Idao.GetQuestionById(id);
            question.VoteUp();
            return View("Question", question);
        }

        public IActionResult DownVote([FromRoute(Name = "id")]int id)
        {
            QuestionModel question = Idao.GetQuestionById(id);
            question.VoteDown();
            return View("Question", question);
        }

        public IActionResult A_UpVote([FromRoute(Name = "id")]int id)
        {
            AnswerModel answer = Idao.GetAnswerByUnique(id);
            answer.VoteUp();
            return View("Question", Idao.GetQuestionById(answer.Question_Id));
        }

        public IActionResult A_DownVote([FromRoute(Name = "id")]int id)
        {
            AnswerModel answer = Idao.GetAnswerByUnique(id);
            answer.VoteDown();
            return View("Question", Idao.GetQuestionById(answer.Question_Id));
        }
    }
}
