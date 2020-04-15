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
            else if (type == "answer")
                Idao.NewAnswer(answer, id);

            return View("Question", Idao.GetQuestionById(id));
        }

        [HttpGet("/delete/{id}")]
        public IActionResult Delete(int id)
        {
            AnswerModel answer = Idao.GetAnswerById(id);
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
            Idao.UpdateVoteNumber(question.Id, question.Vote, "question");

            UserModel user = Idao.GetUserById(question.User_Id);
            user.IncreaseReputation(5);
            Idao.UpdateUser(user);

            return View("Question", question);
        }

        public IActionResult DownVote([FromRoute(Name = "id")]int id)
        {
            QuestionModel question = Idao.GetQuestionById(id);

            question.VoteDown();
            Idao.UpdateVoteNumber(question.Id, question.Vote, "question");

            UserModel user = Idao.GetUserById(question.User_Id);
            user.DecreaseReputation(2);
            Idao.UpdateUser(user);

            return View("Question", question);
        }

        //-ANSWER--------------------------------------------------------------------------------------------------------

        public IActionResult A_UpVote([FromRoute(Name = "id")]int id)
        {
            AnswerModel answer = Idao.GetAnswerById(id);

            answer.VoteUp();
            Idao.UpdateVoteNumber(answer.Id, answer.Vote, "answer");

            UserModel user = Idao.GetUserById(answer.User_Id);
            user.IncreaseReputation(10);
            Idao.UpdateUser(user);

            return View("Question", Idao.GetQuestionById(answer.Question_Id));
        }

        public IActionResult A_DownVote([FromRoute(Name = "id")]int id)
        {
            AnswerModel answer = Idao.GetAnswerById(id);

            answer.VoteDown();
            Idao.UpdateVoteNumber(answer.Id, answer.Vote, "answer");

            UserModel user = Idao.GetUserById(answer.User_Id);
            user.DecreaseReputation(2);
            Idao.UpdateUser(user);

            return View("Question", Idao.GetQuestionById(answer.Question_Id));
        }

        [HttpPost]
        public IActionResult AddComment(int id, [FromForm(Name = "comment")] string comment, [FromForm(Name = "type")] string type)
        {
            if (type == "answer")
            {
                Idao.NewComment(null, id, comment);
                return View("Question", Idao.GetQuestionById(Idao.GetAnswerById(id).Question_Id));
            }
            else
            {
                Idao.NewComment(id, null, comment);
                return View("Question", Idao.GetQuestionById(id));
            }
        }

        [HttpGet("/edit_answer/{id}")]
        public IActionResult Edit_Answer(int id)
        {
            return View("Answer_Edit", Idao.GetAnswerById(id));
        }

        [HttpPost]
        public IActionResult AnswerEdit([FromForm(Name = "content")] string content, [FromForm(Name = "img")] string img, [FromForm(Name = "id")] int id)
        {
            string link = img == null ? Idao.GetAnswerById(id).ImgLink : img;
            Idao.UpdateAnswer(id, content, link);
            return View("Question", Idao.GetQuestionById(Idao.GetAnswerById(id).Question_Id));
        }

        //Search
        [HttpGet]
        public IActionResult Search(string search)
        {
            Idao.SearchText = search;
            //Idao.SearchEntries();
            return View("Index", Idao);
        }

        //-COMMENT--------------------------------------------------------------------------------------------------------

        [HttpGet("/delete_comment/{id}")]
        public IActionResult Delete_Comment(int id)
        {
            QuestionModel question = Idao.GetQuestionByCommentId(id);
            Idao.Delete(id, "comment");
            return View("Question", question);
        }

        [HttpGet("/edit_comment/{id}")]
        public IActionResult Edit_Comment(int id)
        {
            CommentModel instance = null;

            foreach (QuestionModel question in Idao.GetQuestions())
            {
                if (question.GetCommentById(id) != null)
                {
                    instance = question.GetCommentById(id);
                    break;
                }


                foreach (AnswerModel answer in question.Answers)
                {
                    if (answer.GetCommentById(id) != null)
                    {
                        instance = answer.GetCommentById(id);
                        break;
                    }
                }
            }

            return View("Comment_Edit", instance);
        }

        [HttpPost]
        public IActionResult CommentEdit([FromForm(Name = "message")] string message, [FromForm(Name = "id")] int id)
        {
            Idao.UpdateComment(id, message);
            return View("Question", Idao.GetQuestionByCommentId(id));
        }

        public IActionResult Tags()
        {
            return View(Idao.Tags);
        }
    }
}
