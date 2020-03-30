using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskMate.Models
{
    public class QuestionTagModel
    {
        public int QuestionID { get; set; }
        public int TagID { get; set; }

        public QuestionTagModel(int qid, int tagid)
        {
            QuestionID = qid;
            TagID = tagid;
        }
    }
}
