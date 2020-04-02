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
        public IActionResult Sorting([FromForm(Name = "order")] string order)
        {
            Idao.SortQuestion(order);
            return View("Index", Idao);
        }

        [HttpPost]
        public IActionResult ShowEntries([FromForm(Name = "entry")] string entry)
        {
            if (!int.TryParse(entry, out int x))
                Idao.Entry = -1;
            else
                Idao.Entry = int.Parse(entry);

            return View("Index", Idao);
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
        public async Task<IActionResult> ImageUpload(IFormFile file, int id, [FromForm(Name = "type")] string type, [FromForm(Name = "answer")] string answer)
        {

            var filePath = "";
            if (file != null && file.Length > 0)
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
                var filename = Path.GetFileName(uniqFileName + "." + file.FileName.Split(".").Last().ToLower());
                string fullPath = uploadPath + filename;

                imagePath = imagePath + @"\";
                filePath = @".." + Path.Combine(imagePath, filename);

                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                ViewData["FileLocation"] = filePath;

                if (type == "question")
                    Idao.AddLinkToTable(filePath, type, id);
                else if (type == "answer")
                {
                    AnswerModel ans = Idao.NewAnswer(answer, id);
                    Idao.AddLinkToTable(filePath, type, ans.Id);
                }

            }
            return View("Question", Idao.GetQuestionById(id));
        }

        [HttpGet("/delete/{id}")]
        public IActionResult Delete(long id)
        {
            AnswerModel answer = Idao.GetAnswerByUnique(id);
            QuestionModel question = Idao.GetQuestionById(answer.Question_Id);
            Idao.Delete(answer.Id, "answer");
            return View("Question", question);
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

        //public ActionResult NewAnswer([FromForm(Name = "answer")] string answer, [FromForm(Name = "id")] int id)
        //{

        //    return View("Question", question);
        //}

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

        //-ANSWER--------------------------------------------------------------------------------------------------------

        public IActionResult A_UpVote([FromRoute(Name = "id")]long id)
        {
            AnswerModel answer = Idao.GetAnswerByUnique(id);
            answer.VoteUp();
            return View("Question", Idao.GetQuestionById(answer.Question_Id));
        }

        public IActionResult A_DownVote([FromRoute(Name = "id")]long id)
        {
            AnswerModel answer = Idao.GetAnswerByUnique(id);
            answer.VoteDown();
            return View("Question", Idao.GetQuestionById(answer.Question_Id));
        }

        [HttpPost]
        public IActionResult AddComment(long id, [FromForm(Name = "comment")] string comment, [FromForm(Name = "type")] string type)
        {
            if (type == "answer")
            {
                Idao.NewComment(-1, Idao.GetAnswerByUnique(id).Id, comment);
                return View("Question", Idao.GetQuestionById(Idao.GetAnswerByUnique(id).Id));
            }
            else
            {
                int newid = Convert.ToInt32(id);
                Idao.NewComment(newid, -1, comment);
                return View("Question", Idao.GetQuestionById(newid));
            }
        }

        [HttpGet("/edit_answer/{id}")]
        public IActionResult Edit_Answer(long id) 
        {
            return View("Answer_Edit", Idao.GetAnswerByUnique(id));
        }

        [HttpPost]
        public IActionResult AnswerEdit([FromForm(Name = "content")] string content, [FromForm(Name = "img")] string img, [FromForm(Name = "id")] long id)
        {
            AnswerModel answer = Idao.GetAnswerByUnique(id);

            if (!answer.Content.Equals(content))
                answer.SetContent(content);

            if (!answer.ImgLink.Equals(img))
                answer.SetImgLink(img);

            return View("Question", Idao.GetQuestionById(answer.Question_Id));
        }
    }
}
