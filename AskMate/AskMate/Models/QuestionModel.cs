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
        public int Views { get; private set;}
        public String Title { get; private set; }
        public String Content { get; private set; }
        public String ImgLink { get; private set; }
        public long Date { get; }
        public List<AnswerModel> Answers { get; } = new List<AnswerModel>();
        public List<CommentModel> Comments { get; } = new List<CommentModel>();

        public QuestionModel(int id, string title, string content, long date)
        {
            Id = id;
            Title = title;
            Content = content;
            Date = date;
        }

        public QuestionModel(int id, string title, string content, long date, string img, int vote, int views)
        {
            Id = id;
            Title = title;
            Content = content;
            Date = date;
            ImgLink = img;
            Vote = vote;
            Views = views;
        }

        public DateTime GetDate() { return new DateTime(1970, 1, 1).AddMilliseconds(double.Parse(Date.ToString())); }

        public void AddImage(string link) { ImgLink = link; Refr(); }
        public void AddAnswer(AnswerModel answer) { Answers.Add(answer); Refr(); }
        public void AddComment(CommentModel comment) { Comments.Add(comment); Refr(); }
        public void VoteUp() { Vote++; Refr(); }
        public void VoteDown() { Vote--; Refr(); }
        public void IncreaseView() { Views++; Refr(); }


        public void SetTitle(string title) { Title = title; Refr(); }
        public void SetContent(string content) { Content = content; Refr(); }
        public void SetImgLink(string link) { ImgLink = link; Refr(); }
        public void DeleteAnswer(AnswerModel answer) { Answers.Remove(answer); Refr(); }
        public void DeleteComment(CommentModel comment) { Comments.Remove(comment); Refr(); }

        public override string ToString()
        {

            return $"{Id};{Title};{Content};{Date};{ImgLink};{Vote};{Views};";
        }

        private void Refr()
        {
            IDAO_Impl.Instance.Refresh();
        }
    }
}
