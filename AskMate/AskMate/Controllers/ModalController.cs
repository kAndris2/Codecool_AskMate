using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AskMate.Models;
using Microsoft.AspNetCore.Mvc;
using AskMate;

namespace AskMate.Controllers
{
    public class ModalController : Controller
    {
        [HttpGet("/Modal/Index/{id}")]
        public ActionResult Modal(int id)
        {
            foreach (QuestionModel item in IDAO_Impl.Instance.GetQuestions())
            {
                if (id.Equals(item.Id))
                    return View("Home/Index", item);
            }
            return null;
        }
    }
}