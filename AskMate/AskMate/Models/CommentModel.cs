using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskMate.Models
{
    public class CommentModel
    {
        public int ID { get; set; }
        public int? QuestionID { get; set; }
        public int? AnswerID { get; set; }
        public String Message { get; set; }
        public long Date { get; set; }
        public int Edited { get; set; }
        public int User_Id { get; set; }

        public CommentModel(int id, int? qid, int? aid, string message, long date, int userid)
        {
            ID = id;
            QuestionID = qid;
            AnswerID = aid;
            Message = message;
            Date = date;
            User_Id = userid;
        }

        public CommentModel(int id, int? qid, int? aid, string message, long date, int edited, int userid)
        {
            ID = id;
            QuestionID = qid;
            AnswerID = aid;
            Message = message;
            Date = date;
            Edited = edited;
            User_Id = userid;
        }

        public void IncreaseEditNumber() { Edited++; }
        public void EditMessage(string message) { Message = message; }
        public DateTime GetDate() { return new DateTime(1970, 1, 1).AddMilliseconds(double.Parse(Date.ToString())); }

        public long GetUnique()
        {
            return Convert.ToInt64($"{ID}{Date}");
        }
    }
}
