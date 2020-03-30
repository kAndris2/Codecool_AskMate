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
        const String FILENAME = "./Resources/Questions.csv";
        const String ANSWER_SEP = "$$";
        const char ANSWER_PROP_SEP = ',';

        List<QuestionModel> Questions = new List<QuestionModel>();
        static IDAO_Impl instance = null;

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
            foreach(QuestionModel question in Questions)
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

        public List<AnswerModel> GetAnswers(int questionId)
        {
            foreach (QuestionModel item in Questions)
            {
                if (questionId.Equals(item.Id))
                    return item.Answers;
            }
            throw new ArgumentException($"Invalid Question ID! ('{questionId}')");
        }

        public QuestionModel GetQuestion(int id)
        {
            foreach (QuestionModel item in Questions)
            {
                if (id.Equals(item.Id))
                    return item;
            }
            throw new ArgumentException($"Invalid Question ID! ('{id}')");
        }

        public List<QuestionModel> GetQuestions()
        {
            return Questions;
        }

        public void NewQuestion(string title, string content)
        {
            
            long milisec = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            int id=0;
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

        /// <summary>
        /// Reload the file with fresh datas.
        /// </summary>
        public void Refresh()
        {
            string text = "";
            foreach (QuestionModel question in Questions)
            {
                text += $"{question.ToString()}{GetFormattedAnswers(question)}";
            }
            File.WriteAllText(FILENAME, text);
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
            foreach (QuestionModel q in Questions)
            {
                if (id.Equals(q.Id))
                {
                    q.AddImage(filePath);
                    break;
                }
            }
            Refresh();
        }

        public void AddLinkToAnswer(string filePath, int id)
        {
            
            foreach (AnswerModel ans in GetAnswers(id))
            {
                if (ans.Id == id)
                {
                    ans.AddImage(filePath);
                    break;
                }
            }
            Refresh();
        }

        public QuestionModel GetQuestionById(int id)
        {
            foreach (QuestionModel question in Questions)
            {
                if (id.Equals(question.Id))
                {
                    return question;
                }
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

        private List<AnswerModel> SetAnswers(string table)
        {
            List<AnswerModel> answers = new List<AnswerModel>();
            string[] data = table.Split(ANSWER_SEP);

            for (int i = 0; i < data.Length; i++)
            {
                string[] temp = data[i].Split(ANSWER_PROP_SEP);
                answers.Add(new AnswerModel(int.Parse(temp[0]),
                                            temp[1],
                                            Convert.ToInt64(temp[2]),
                                            int.Parse(temp[3]),
                                            int.Parse(temp[4]),
                                            temp[5]
                                            )); 
            }
            return answers;
        }

        private String GetFormattedAnswers(QuestionModel question) 
        {
            if (question.Answers.Count == 0)
                return "\n"; 

            string[] props = new string[question.Answers.Count];

            for (int i = 0; i < question.Answers.Count; i++)
            {
                props[i] = question.Answers[i].ToString();
            }
            return string.Join(ANSWER_SEP, props) + "\n";
        }

        /// <summary>
        /// Loads the files from .csv
        /// </summary>
        private void LoadFiles()
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
                            Convert.ToInt64(reader["submission_time"].ToString())
                            )
                        );
                    }
                }
            }
            /*
            if (File.Exists(FILENAME))
            {
                Questions.Clear();
                string[] table = File.ReadAllLines(FILENAME);

                for (int i = 0; i < table.Length; i++)
                {
                    string[] temp = table[i].Split(";");
                    Questions.Add(new QuestionModel(int.Parse(temp[0]),
                                                    temp[1],
                                                    temp[2],
                                                    Convert.ToInt64(temp[3]),
                                                    temp[4],
                                                    int.Parse(temp[5]),
                                                    int.Parse(temp[6]),
                                                    temp[6] != "" ? SetAnswers(temp[7]) : new List<AnswerModel>()
                                                    ));
                }
            }
            */
        }
    }
}
