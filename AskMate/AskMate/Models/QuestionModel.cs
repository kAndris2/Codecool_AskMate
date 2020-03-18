using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskMate.Models
{
    public class QuestionModel
    {
        public int Id { get; }
        public int Vote { get; private set; }
        public String Title { get; private set; }
        public String Content { get; private set; }
        public String ImgLink { get; private set; }
        public DateTime Date { get; }
        public List<AnswerModel> Answers { get; } = new List<AnswerModel>();

        public QuestionModel(int id, string title, string content, string time)
        {
            Id = id;
            Title = title;
            Content = content;
            Date = new DateTime(1970, 1, 1).AddMilliseconds(double.Parse(time));
        }

        public void AddImage(string link) { ImgLink = link; }
        public void AddAnswer(AnswerModel answer) { Answers.Add(answer); }
        public void VoteUp() { Vote++; }
        public void VoteDown() { Vote--; }
        public void SetTitle(string title) { Title = title; }
        public void SetContent(string content) { Content = content; }
        public void SetImgLink(string link) { ImgLink = link; }
    }
}
