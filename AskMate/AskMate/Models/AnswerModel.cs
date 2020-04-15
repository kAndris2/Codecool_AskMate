using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskMate.Models
{
    public class AnswerModel
    {
        public int Id { get; }
        public int Vote { get; private set; }
        public int Question_Id { get; private set; }
        public int User_Id { get; private set; }
        public String Content { get; private set; }
        public String ImgLink { get; private set; }
        public long Date { get; }
        public List<CommentModel> Comments { get; } = new List<CommentModel>();

        public AnswerModel(int id, int qid, string content, long date, int userid)
        {
            Id = id;
            Question_Id = qid;
            Content = content;
            Date = date;
            User_Id = userid;
        }

        public AnswerModel(int id, string content, long date, int vote, int qid, string link, int userid)
        {
            Id = id;
            Content = content;
            Date = date;
            Vote = vote;
            ImgLink = link;   //HARDCODED
            Question_Id = qid;
            User_Id = userid;
        }

        public DateTime GetDate() { return new DateTime(1970, 1, 1).AddMilliseconds(double.Parse(Date.ToString())); }
        public void AddImage(string link) { ImgLink = link; }
        public void VoteUp() { Vote++; }
        public void VoteDown() { Vote--; }
        public void AddComment(CommentModel comment) { Comments.Add(comment); }
        public void DeleteComment(CommentModel comment) { Comments.Remove(comment); }
        public List<CommentModel> GetComments() { return Comments; }
        public void SetContent(string content) { Content = content; }
        public void SetImgLink(string imglink) { ImgLink = imglink; }

        public CommentModel GetCommentById(int id)
        {
            CommentModel instance = null;
            foreach (CommentModel comment in Comments)
            {
                if (id.Equals(comment.ID))
                {
                    instance = comment;
                    break;
                }
            }
            return instance;
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
