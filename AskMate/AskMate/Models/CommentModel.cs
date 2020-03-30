using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskMate.Models
{
    public class CommentModel
    {
        public int ID { get; set; }
        public int QuestionID { get; set; }
        public int AnswerID { get; set; }
        public String Message { get; set; }
        public long Date { get; set; }
        public int Edited { get; set; }

        public CommentModel(int id, int qid, int aid, string message, long date)
        {
            ID = id;
            QuestionID = qid;
            AnswerID = aid;
            Message = message;
            Date = date;
        }

        public void IncreaseEditNumber() { Edited++; }
    }
}
