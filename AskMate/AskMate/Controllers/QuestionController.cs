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
            var iDAO = IDAO_Impl.Instance;

            string Title = newQuestion.Title;
            string Content = newQuestion.Content;
            string ownTags = newQuestion.ownTags;
            List<string> tags;
            tags = ownTags.Split(',').ToList();
            List<TagModel> newTags = new List<TagModel>();

            foreach (string tag in tags)
            {
                TagModel tagModel = iDAO.Tags.Find(t => t.Name == tag);
                if (tagModel == null)    //Check if tag doesn't exist
                {
                    newTags.Add(IDAO_Impl.Instance.CreateTag(tag));
                }
                else
                {
                    newTags.Add(tagModel);
                }
            }

            iDAO.NewQuestion(Title, Content, newTags);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Question()
        {
            return View();
        }
    }
}