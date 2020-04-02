using Npgsql;
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
        public int Views { get; private set;}
        public String Title { get; private set; }
        public String Content { get; private set; }
        public String ImgLink { get; private set; }
        public long Date { get; }
        public List<AnswerModel> Answers { get; } = new List<AnswerModel>();
        public List<CommentModel> Comments { get; } = new List<CommentModel>();
        public List<QuestionTagModel> ownTags { get; } = new List<QuestionTagModel>();
        public List<TagModel> tags { get; set; } = new List<TagModel>();

        public List<string> tagNames { get; private set; } = new List<string>();
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

        public void AddImage(string link) { ImgLink = link; }
        public void AddAnswer(AnswerModel answer) { Answers.Add(answer); }
        public void AddComment(CommentModel comment) { Comments.Add(comment); }
        public void VoteUp() { Vote++; }
        public void VoteDown() { Vote--; }
        public void AddNewTag(QuestionTagModel newtag) { ownTags.Add(newtag); } //set tag to question
        public void GetTag()
        {

            using (var conn = new NpgsqlConnection(Program.ConnectionString))
            {
                //string sqlstr = "";
                foreach (QuestionTagModel tag in ownTags)
                {
                    string sqlstr = "select * from tag where id = @id";
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(sqlstr, conn))
                    {
                        cmd.Parameters.AddWithValue("id", tag.TagID);
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            tagNames.Add(reader["name"].ToString());
                        }
                    }
                    conn.Close();
                }
            }
        }//get the name of the tag(s)

        public void IncreaseView() 
        { 
            Views++;
            IDAO_Impl.Instance.UpdateQuestionView(this);
        }


        public void SetTitle(string title) { Title = title; }
        public void SetContent(string content) { Content = content; }
        public void DeleteAnswer(AnswerModel answer) { Answers.Remove(answer); }
        public void DeleteComment(CommentModel comment) { Comments.Remove(comment); }

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
            return $"{Id};{Title};{Content};{Date};{ImgLink};{Vote};{Views};";
        }
    }
}
