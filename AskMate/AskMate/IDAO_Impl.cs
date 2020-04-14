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
        List<TagModel> Tags = new List<TagModel>();
        List<QuestionTagModel> QuestionTags = new List<QuestionTagModel>();
        public List<UserModel> Users = new List<UserModel>();

        public int Entry { get; set; } = 5;
        public string SearchText { get; set; }
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

        public AnswerModel GetAnswerById(int id)
        {
            AnswerModel instance = null;
            foreach (QuestionModel question in Questions)
            {
                foreach (AnswerModel answer in question.Answers)
                {
                    if (id.Equals(answer.Id))
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
            List<QuestionModel> questions;
            if (SearchText == null)
            {
                questions = Questions.ToList();
                /* System.Console.WriteLine("added empty"); */
            }
            else
            {
                questions = new List<QuestionModel>();
                /* System.Console.WriteLine("added cuz contains smthing");
                System.Console.WriteLine("this:" + SearchText); */
                foreach (var question in Questions)
                {
                    if (question.Title.Contains(SearchText))
                    {
                        questions.Add(question);
                    }
                    else if (question.Content.Contains(SearchText))
                    {
                        questions.Add(question);
                    }
                    else
                    {
                        foreach (var answer in question.Answers)
                        {
                            if (answer.Content.Contains(SearchText))
                            {
                                questions.Add(question);
                            }
                        }
                    }
                }
            }

            if (Entry == -1)
            {
                return questions;
            }
            else
            {
                List<QuestionModel> _questions = new List<QuestionModel>();
                for (int i = questions.Count - 1; i >= 0; i--)
                {
                    _questions.Add(questions[i]);
                    if (_questions.Count == Entry)
                        break;
                }
                return _questions;
            }
        }

        private void RemoveCommentFrom(int id)
        {
            CommentModel instance = null;
            foreach (QuestionModel question in Questions)
            {
                if (question.GetCommentById(id) != null)
                {
                    instance = question.GetCommentById(id);
                    question.DeleteComment(instance);
                    break;
                }

                foreach (AnswerModel answer in question.Answers)
                {
                    if (answer.GetCommentById(id) != null)
                    {
                        instance = answer.GetCommentById(id);
                        answer.DeleteComment(instance);
                        break;
                    }
                }
            }
        }

        public QuestionModel GetQuestionByCommentId(int id)
        {
            foreach (QuestionModel question in Questions)
            {
                if (question.GetCommentById(id) != null)
                    return question;

                foreach (AnswerModel answer in question.Answers)
                {
                    if (answer.GetCommentById(id) != null)
                        return question;
                }
            }
            throw new ArgumentException($"Invalid comment ID! - ('{id}')");
        }

        private void AddCommentTo(CommentModel comment)
        {
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

        private CommentModel GetCommentById(int id)
        {
            foreach (QuestionModel question in Questions)
            {
                if (question.GetCommentById(id) != null)
                    return question.GetCommentById(id);

                foreach (AnswerModel answer in question.Answers)
                {
                    if (answer.GetCommentById(id) != null)
                        return answer.GetCommentById(id);
                }
            }
            throw new ArgumentException($"Invalid comment ID! - ('{id}')");
        }

        //-SQL_METHODS---------------------------------------------------------------------------------------------------

        public TagModel CreateTag(string tag)
        {
            string sqlstr = "INSERT INTO tag (name) VALUES (@tag)";
            int id = 0;
            using (var conn = new NpgsqlConnection(Program.ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sqlstr, conn))
                {
                    cmd.Parameters.AddWithValue("tag", tag);
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new NpgsqlCommand("SELECT * FROM tag", conn))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        id = int.Parse(reader["id"].ToString());
                    }
                }
            }
            TagModel nTag = new TagModel(id, tag);
            Tags.Add(nTag);
            return nTag;
        }


        public void Delete(int id, string table)
        {
            using (var conn = new NpgsqlConnection(Program.ConnectionString))
            {
                conn.Open();
                string sqlstr = $"DELETE FROM {table} " +
                                $"WHERE id = {id};";
                var cmd = new NpgsqlCommand(sqlstr, conn);
                cmd.ExecuteNonQuery();
            }

            if (table == "answer")
            {
                AnswerModel answer = GetAnswerById(id);
                GetQuestionById(answer.Question_Id).DeleteAnswer(answer);
            }
            else if (table == "question")
            {
                Questions.Remove(GetQuestionById(id));
            }
            else if (table == "comment")
            {
                RemoveCommentFrom(id);
            }
        }

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
                        questions.Add(GetQuestionById(int.Parse(reader["id"].ToString())));
                    }
                }
            }
            Questions.Clear();
            Questions = questions;
        }


        public void UpdateAnswer(int id, string content, string img)
        {
            AnswerModel answer = GetAnswerById(id);
            answer.SetContent(content);
            answer.SetImgLink(img);

            string sqlstr = "UPDATE answer " +
                            "SET message = @message, image = @image " +
                            "WHERE id = @id";
            using (var conn = new NpgsqlConnection(Program.ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sqlstr, conn))
                {
                    cmd.Parameters.AddWithValue("message", answer.Content);
                    cmd.Parameters.AddWithValue("image", answer.ImgLink);
                    cmd.Parameters.AddWithValue("id", answer.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateQuestionView(QuestionModel question)
        {
            string sqlstr = "UPDATE question " +
                            "SET view_number = @view " +
                            "WHERE id = @id";
            using (var conn = new NpgsqlConnection(Program.ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sqlstr, conn))
                {
                    cmd.Parameters.AddWithValue("view", question.Views);
                    cmd.Parameters.AddWithValue("id", question.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateVoteNumber(int id, int number, string table)
        {
            string sqlstr = $"UPDATE {table} " +
                            "SET vote_number = @vote " +
                            "WHERE id = @id";
            using (var conn = new NpgsqlConnection(Program.ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sqlstr, conn))
                {
                    cmd.Parameters.AddWithValue("vote", number);
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateComment(int id, string message)
        {
            CommentModel comment = GetCommentById(id);

           long milisec = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            string sqlstr = $"UPDATE comment " +
                            "SET message = @message, submission_time = @date " +
                            "WHERE id = @id";
            using (var conn = new NpgsqlConnection(Program.ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sqlstr, conn))
                {
                    cmd.Parameters.AddWithValue("message", message);
                    cmd.Parameters.AddWithValue("date", milisec);
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            comment.Message = message;
            comment.Date = milisec;
        }

        public void EditLine(int id, string title, string content)
        {
            string sqlstr = "UPDATE question SET title = @title, message = @message WHERE id = @id";
            bool found = false;
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
                    found = true;
                    break;
                }
            }

            if (!found)
                throw new Exception("No such id!");
        }

        public void NewComment(int? qid, int? aid, string content)
        {
            long milisec = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            int id = 0;
            string sqlstr = "INSERT INTO comment " +
                                "(question_id,answer_id,message,submission_time) " +
                            "VALUES " +
                                "(@question_id,@answer_id,@message,@time)";
            using (var conn = new NpgsqlConnection(Program.ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sqlstr, conn))
                {
                    if (qid is null)
                        cmd.Parameters.AddWithValue("question_id", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("question_id", qid);

                    if (aid is null)
                        cmd.Parameters.AddWithValue("answer_id", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("answer_id", aid);

                    cmd.Parameters.AddWithValue("message", content);
                    cmd.Parameters.AddWithValue("time", milisec);
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new NpgsqlCommand("SELECT * FROM comment", conn))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        id = int.Parse(reader["id"].ToString());
                    }
                }
            }
            AddCommentTo(new CommentModel(id, qid, aid, content, milisec));
        }

        public AnswerModel NewAnswer(string content, int question_id)
        {
            long milisec = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            int id = 0;
            string sqlstr = "INSERT INTO answer " +
                                "(submission_time,question_id,message) " +
                            "VALUES " +
                                "(@time,@question_id,@message)";
            using (var conn = new NpgsqlConnection(Program.ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sqlstr, conn))
                {
                    cmd.Parameters.AddWithValue("time", milisec);
                    cmd.Parameters.AddWithValue("question_id", question_id);
                    cmd.Parameters.AddWithValue("message", content);
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new NpgsqlCommand("SELECT * FROM answer", conn))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        id = int.Parse(reader["id"].ToString());
                    }
                }
            }
            AnswerModel answer = new AnswerModel(id, question_id, content, milisec);
            GetQuestionById(question_id).AddAnswer(answer);
            return answer;
        }

        public void NewQuestion(string title, string content, List<TagModel> newTags)
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

            for (int i = 0; i < newTags.Count; i++)
            {
                QuestionTagModel qTag = new QuestionTagModel(0, 0);
                qTag.NewTagToQuestionTag(newTags[i], question);
                question.AddNewTag(qTag);
            }
            question.GetTag();
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
            string delqutag = "DELETE FROM question_tag WHERE question_id = @id";
            string sqlstr = "DELETE FROM question WHERE id = @id";
            using (var conn = new NpgsqlConnection(Program.ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(delqutag, conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.ExecuteNonQuery();
                }
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

        public void AddLinkToTable(string filePath, string table, int id)
        {
            string sqlstr = $"UPDATE {table}" +
                            " SET image = @image " +
                            "WHERE id = @id";
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

            if (table == "question")
            {
                foreach (QuestionModel question in Questions)
                {
                    if (id.Equals(question.Id))
                    {
                        question.AddImage(filePath);
                        break;
                    }
                }
            }
            else if (table == "answer")
            {
                foreach (QuestionModel question in Questions)
                {
                    foreach (AnswerModel answer in question.Answers)
                    {
                        if (id.Equals(answer.Id))
                        {
                            answer.AddImage(filePath);
                            break;
                        }
                    }
                }
            }
        }
        public void GetTags()
        {
            string sqlstr = "select * from tag";
            using (var conn = new NpgsqlConnection(Program.ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sqlstr, conn))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Tags.Add(new TagModel(int.Parse(reader["id"].ToString()), reader["name"].ToString()));
                    }
                }
            }

        }
        public void GetQuestionTags()
        {
            string sqlstr = "select * from question_tag";

            using (var conn = new NpgsqlConnection(Program.ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sqlstr, conn))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        QuestionTags.Add(new QuestionTagModel(int.Parse(reader["question_id"].ToString()), int.Parse(reader["tag_id"].ToString())));
                    }
                }
            }
        }

        public void SetTagsToQuestion()
        {
            GetTags();
            GetQuestionTags();
            foreach (QuestionModel question in Questions)
            {
                foreach (QuestionTagModel qtag in QuestionTags)
                {
                    if (qtag.QuestionID == question.Id)
                    {
                        question.AddNewTag(qtag);

                    }
                }
                question.GetTag();
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
            SetTagsToQuestion();

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

                        GetQuestionById(answer.Question_Id).AddAnswer(answer);
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
                        int? aid, qid;

                        if (!int.TryParse(reader["question_id"].ToString(), out int x))
                            qid = null;
                        else
                            qid = int.Parse(reader["question_id"].ToString());

                        if (!int.TryParse(reader["answer_id"].ToString(), out int y))
                            aid = null;
                        else
                            aid = int.Parse(reader["answer_id"].ToString());

                        CommentModel comment = new CommentModel
                        (
                            int.Parse(reader["id"].ToString()),
                            qid,
                            aid,
                            reader["message"].ToString(),
                            Convert.ToInt64(reader["submission_time"].ToString()),
                            int.Parse(reader["edited_number"].ToString())
                        );
                        AddCommentTo(comment);
                    }
                }
            }
        }
    }
}
