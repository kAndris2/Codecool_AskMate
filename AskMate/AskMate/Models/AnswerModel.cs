using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AskMate;

namespace AskMate.Models
{
    public class AnswerModel
    {
        public int Id { get; }
        public int Vote { get; private set; }
        public int Question_Id { get; private set; }
        public String Content { get; private set; }
        public String ImgLink { get; private set; }
        public long Date { get; }
        public List<CommentModel> Comments { get; } = new List<CommentModel>();

        public AnswerModel(int id, int qid, string content, long data)
        {
            Id = id;
            Question_Id = qid;
            Content = content;
            Date = data;
        }
        
        public AnswerModel(int id, string content, long date, int vote, int qid, string link)
        {
            Id = id;
            Content = content;
            Date = date;
            Vote = vote;
            ImgLink = link;   //HARDCODED
            Question_Id = qid;
        }

        public DateTime GetDate() { return new DateTime(1970, 1, 1).AddMilliseconds(double.Parse(Date.ToString())); }
        public void AddImage(string link) { ImgLink = link; Refr(); }
        public void VoteUp() { Vote++; Refr(); }
        public void VoteDown() { Vote--; Refr(); }
        public void AddComment(CommentModel comment) { Comments.Add(comment); Refr(); }
        public void DeleteComment(CommentModel comment) { Comments.Remove(comment); Refr(); }

        public long GetUnique()
        {
            return Convert.ToInt64($"{Id}{Date}");
        }

        private void Refr()
        {
            IDAO_Impl.Instance.Refresh();
        }

        public override string ToString()
        {
            return $"{Id}," +
                   $"{Content}," +
                   $"{Date}," +
                   $"{Vote}," +
                   $"{Question_Id}," +
                   $"{(ImgLink == "" ? "N/A" : ImgLink)}";
        }
    }
}
