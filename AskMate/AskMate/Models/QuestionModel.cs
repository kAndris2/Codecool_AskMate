using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskMate.Models
{
    public class QuestionModel
    {
        public int Id { get; set; }
        public int Vote { get; set; }
        public String Title { get; set; }
        public String Content { get; set; }
        public String ImgLink { get; set; }
        public DateTime Date { get; set; }
        public float Rate { get; set; }
        public List<AnswerModel> Answers { get; set; }

        public QuestionModel(int id, string title, string content, string link)
        {
            Id = id;
            Title = title;
            Content = content;
            ImgLink = link;
            Date = DateTime.Now;
        }

        public void AddImage(string link)
        {
            ImgLink = link;
        }

        
        public void AddAnswer(AnswerModel answer)
        {
            Answers.Add(answer);
        }

        public List<AnswerModel> GetAnswers()
        {
            return Answers;
        }
    }
}
