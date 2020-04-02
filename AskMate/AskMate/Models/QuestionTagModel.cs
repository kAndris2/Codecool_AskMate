using Npgsql;
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
        
        public void NewTagToQuestionTag(TagModel tag, QuestionModel question)
        {
            TagID = tag.ID;
            QuestionID = question.Id;
            ToDB();
        }
        public void ToDB()
        {
            string sqlstr = "insert into question_tag (question_id,tag_id) values (@qid,@tid)";
            using (var conn = new NpgsqlConnection(Program.ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sqlstr, conn))
                {
                    cmd.Parameters.AddWithValue("qid", QuestionID);
                    cmd.Parameters.AddWithValue("tid", TagID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}
