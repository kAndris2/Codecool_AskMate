using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AskMate;

namespace AskMate.Models
{
    public class QuestionModel
    {
        public int Id { get; }
        public int Vote { get; private set; }
        public String Title { get; private set; }
        public String Content { get; private set; }
        public String ImgLink { get; private set; }
        public long Date { get; }
        public List<AnswerModel> Answers { get; } = new List<AnswerModel>();

        public QuestionModel(int id, string title, string content, long date, string link, int vote, List<AnswerModel> answers)
        {
            Id = id;
            Title = title;
            Content = content;
            Date = date;
            ImgLink = link;
            Vote = vote;
            Answers = answers;
        }
        public DateTime GetDate() { return new DateTime(1970, 1, 1).AddMilliseconds(double.Parse(Date.ToString())); }

        public void AddImage(string link) { ImgLink = link; Refr(); }
        public void AddAnswer(AnswerModel answer) { Answers.Add(answer); Refr(); }
        public void VoteUp() { Vote++; Refr(); }
        public void VoteDown() { Vote--; Refr(); }


        public void SetTitle(string title) { Title = title; Refr(); }
        public void SetContent(string content) { Content = content; Refr(); }
        public void SetImgLink(string link) { ImgLink = link; Refr(); }

        public override string ToString()
        {

            return $"{Id};{Title};{Content};{Date};{ImgLink};{Vote};";
        }

        private void Refr()
        {
            IDAO_Impl.Instance.Refresh();
        }
    }
}
