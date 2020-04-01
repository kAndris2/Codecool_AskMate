using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using AskMate.Models;
using Npgsql;

namespace AskMate
{
    public sealed class IDAO_Impl : IDAO
    {
        static IDAO_Impl instance = null;
        List<QuestionModel> Questions = new List<QuestionModel>();
        public int Entry { get; set; } = 5;
        private Dictionary<string, bool> Sort = new Dictionary<string, bool>
        {
            { "id", true },
            { "title", true },
            { "submission_time", true },
            { "message", true },
            { "view_number", true },
            { "vote_number", true }
        };

        public static IDAO_Impl Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new IDAO_Impl();
                }
                return instance;
            }
        }

        private IDAO_Impl()
        {
            LoadFiles();
        }

        public List<AnswerModel> GetAnswers(int questionId)
        {
            foreach (QuestionModel item in Questions)
            {
                if (questionId.Equals(item.Id))
                    return item.Answers;
            }
            throw new ArgumentException($"Invalid Question ID! ('{questionId}')");
        }

        public List<QuestionModel> GetQuestions()
        {
            return Questions;
        }

        public QuestionModel GetQuestionById(int id)
        {
            foreach (QuestionModel question in Questions)
            {
                if (id.Equals(question.Id))
                    return question;
            }
            throw new ArgumentException($"Invalid Question ID! ('{id}')");
        }

        /// <summary>
        /// Get answer by it's unique code.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AnswerModel GetAnswerByUnique(long id)
        {
            AnswerModel instance = null;

            foreach (QuestionModel question in Questions)
            {
                foreach (AnswerModel answer in question.Answers)
                {
                    if (id.Equals(answer.GetUnique()))
                    {
                        instance = answer;
                        break;
                    }
                }
            }
            return instance;
        }

        public List<QuestionModel> GetEntries()
        {
            if (Entry == -1)
                return Questions;

            List<QuestionModel> questions = new List<QuestionModel>();

            for (int i = Questions.Count - 1; i >= 0; i--)
            {
                questions.Add(Questions[i]);
                if (questions.Count == Entry)
                    break;
            }
            return questions;
        }

        //-SQL_METHODS---------------------------------------------------------------------------------------------------

        public void SortQuestion(string order)
        {
            List<QuestionModel> questions = new List<QuestionModel>();
            Sort[order] = !Sort[order];

            using (var conn = new NpgsqlConnection(Program.ConnectionString))
            {
                conn.Open();
                string sqlstr = "SELECT * FROM question " +
                                $"ORDER BY {order} " +
                                $"{(Sort[order] == true ? "ASC" : "DESC")}";
                using (var cmd = new NpgsqlCommand(sqlstr, conn))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        questions.Add
                        (
                            new QuestionModel
                            (
                            int.Parse(reader["id"].ToString()),
                            reader["title"].ToString(),
                            reader["message"].ToString(),
                            Convert.ToInt64(reader["submission_time"].ToString()),
                            reader["image"].ToString(),
                            int.Parse(reader["vote_number"].ToString()),
                            int.Parse(reader["view_number"].ToString())
                            )
                        );
                    }
                }
            }
            Questions.Clear();
            Questions = questions;
        }

        public void EditLine(int id, string title, string content)
        {
            string sqlstr = "UPDATE question SET title = @title, message = @message WHERE id = @id";
            using (var conn = new NpgsqlConnection(Program.ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sqlstr, conn))
                {
                    cmd.Parameters.AddWithValue("title", title);
                    cmd.Parameters.AddWithValue("message", content);
                    cmd.Parameters.AddWithValue("id", id);

                    cmd.ExecuteNonQuery();
                }
            }
            foreach (QuestionModel question in Questions)
            {
                if (question.Id == id)
                {
                    question.SetTitle(title);
                    question.SetContent(content);

                }
                else
                {
                    throw new Exception("No id ");
                }
            }
        }

        public void NewQuestion(string title, string content)
        {

            long milisec = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            int id = 0;
            string sqlstr = "INSERT INTO question (submission_time,view_number,vote_number,title,message) VALUES (@time,@views,@votes,@title,@message)";
            using (var conn = new NpgsqlConnection(Program.ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sqlstr, conn))
                {
                    cmd.Parameters.AddWithValue("time", milisec);
                    cmd.Parameters.AddWithValue("views", 0);
                    cmd.Parameters.AddWithValue("votes", 0);
                    cmd.Parameters.AddWithValue("title", title);
                    cmd.Parameters.AddWithValue("message", content);
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new NpgsqlCommand("SELECT * FROM question", conn))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        id = int.Parse(reader["id"].ToString());
                    }
                }
            }
            QuestionModel question = new QuestionModel(id, title, content, milisec);
            Questions.Add(question);
        }

        public void QuestionRefresh(QuestionModel question)
        {
            string sqlstr = "UPDATE question " +
                            "SET " +
                                "title = @title," +
                                "message = @message," +
                                "image = @image," +
                                "vote_number = @vote_number," +
                                "view_number = @view_number" +
                            " WHERE id = @id";
            using (var conn = new NpgsqlConnection(Program.ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sqlstr, conn))
                {
                    cmd.Parameters.AddWithValue("title", question.Title);
                    cmd.Parameters.AddWithValue("message", question.Content);
                    cmd.Parameters.AddWithValue("image", question.ImgLink);
                    cmd.Parameters.AddWithValue("vote_number", question.Vote);
                    cmd.Parameters.AddWithValue("view_number", question.Views);
                    cmd.Parameters.AddWithValue("id", question.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void AnswerRefresh(AnswerModel answer)
        {
            string sqlstr = "UPDATE answer " +
                            "SET " +
                                "vote_number = @vote_number," +
                                "question_id = @question_id," +
                                "message = @message," +
                                "image = @image" +
                            " WHERE id = @id";
            using (var conn = new NpgsqlConnection(Program.ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sqlstr, conn))
                {
                    cmd.Parameters.AddWithValue("vote_number", answer.Vote);
                    cmd.Parameters.AddWithValue("question_id", answer.Question_Id);
                    cmd.Parameters.AddWithValue("message", answer.Content);
                    cmd.Parameters.AddWithValue("image", answer.ImgLink);
                    cmd.Parameters.AddWithValue("id", answer.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void CommentRefresh(CommentModel comment)
        {
            string sqlstr = "UPDATE comment " +
                            "SET " +
                                "question_id = @question_id," +
                                "answer_id = @answer_id," +
                                "message = @message," +
                                "edited_number = @edited_number" +
                            " WHERE id = @id";
            using (var conn = new NpgsqlConnection(Program.ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sqlstr, conn))
                {
                    cmd.Parameters.AddWithValue("question_id", comment.QuestionID);
                    cmd.Parameters.AddWithValue("answer_id", comment.AnswerID);
                    cmd.Parameters.AddWithValue("message", comment.Message);
                    cmd.Parameters.AddWithValue("edited_number", comment.Edited);
                    cmd.Parameters.AddWithValue("id", comment.ID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteQuestion(int id)
        {
            string sqlstr = "DELETE FROM question WHERE id = @id";
            using (var conn = new NpgsqlConnection(Program.ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sqlstr, conn))
                {
                    cmd.Parameters.AddWithValue("id", id);

                    cmd.ExecuteNonQuery();
                }
            }

            foreach (QuestionModel question in Questions)
            {
                if (id.Equals(question.Id))
                {
                    Questions.Remove(question);
                    break;
                }
            }

        }

        public void AddLinkToQuestion(string filePath, int id)
        {
            string sqlstr = "UPDATE question SET image = @image WHERE id = @id";
            using (var conn = new NpgsqlConnection(Program.ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sqlstr, conn))
                {
                    cmd.Parameters.AddWithValue("image", filePath);
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.ExecuteNonQuery();
                }
            }

            foreach (QuestionModel q in Questions)
            {
                if (id.Equals(q.Id))
                {
                    q.AddImage(filePath);
                    QuestionRefresh(q);
                    break;
                }
            }
        }

        public void AddLinkToAnswer(string filePath, int id)
        {

            foreach (AnswerModel ans in GetAnswers(id))
            {
                if (ans.Id == id)
                {
                    ans.AddImage(filePath);
                    AnswerRefresh(ans);
                    break;
                }
            }
        }

        /// <summary>
        /// Loads the files from db
        /// </summary>
        public void LoadFiles()
        {
            using (var conn = new NpgsqlConnection(Program.ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM question", conn))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Questions.Add
                        (
                            new QuestionModel
                            (
                            int.Parse(reader["id"].ToString()),
                            reader["title"].ToString(),
                            reader["message"].ToString(),
                            Convert.ToInt64(reader["submission_time"].ToString()),
                            reader["image"].ToString(),
                            int.Parse(reader["vote_number"].ToString()),
                            int.Parse(reader["view_number"].ToString())
                            )
                        );
                    }
                }
            }
            using (var conn = new NpgsqlConnection(Program.ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT * FROM answer", conn))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        AnswerModel answer = new AnswerModel
                        (
                            int.Parse(reader["id"].ToString()),
                            reader["message"].ToString(),
                            Convert.ToInt64(reader["submission_time"].ToString()),
                            int.Parse(reader["vote_number"].ToString()),
                            int.Parse(reader["question_id"].ToString()),
                            reader["image"].ToString()
                        );

                        foreach (QuestionModel question in Questions)
                        {
                            if (question.Id.Equals(answer.Question_Id))
                            {
                                question.AddAnswer(answer);
                                break;
                            }
                        }
                    }
                }
            }
            using (var conn = new NpgsqlConnection(Program.ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT * FROM comment", conn))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        CommentModel comment = new CommentModel
                        (
                            int.Parse(reader["id"].ToString()),
                            int.Parse(reader["question_id"].ToString()),
                            int.Parse(reader["answer_id"].ToString()),
                            reader["messafe"].ToString(),
                            Convert.ToInt64(reader["submission_time"].ToString()),
                            int.Parse(reader["edited_number"].ToString())
                        );

                        foreach (QuestionModel question in Questions)
                        {
                            if (question.Id.Equals(comment.QuestionID))
                            {
                                question.AddComment(comment);
                                break;
                            }
                            else
                            {
                                foreach (AnswerModel answer in question.Answers)
                                {
                                    if (answer.Id.Equals(comment.AnswerID))
                                    {
                                        answer.AddComment(comment);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
