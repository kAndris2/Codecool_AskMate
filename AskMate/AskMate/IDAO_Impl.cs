using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using AskMate.Models;

namespace AskMate
{
    public sealed class IDAO_Impl : IDAO
    {
        const String FILENAME = "./Resources/Questions.csv";
        const String ANSWER_SEP = "$$";
        const char ANSWER_PROP_SEP = '.';

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
            string[] table = File.ReadAllLines(FILENAME);

            for (int i = 0; i < table.Length; i++)
            {
                string[] temp = table[i].Split(";");
                List<AnswerModel> answers = SetAnswers(temp[6]);
                Questions.Add(new QuestionModel(int.Parse(temp[0]), 
                                                temp[1], 
                                                temp[2], 
                                                Convert.ToInt64(temp[3]), 
                                                temp[4], 
                                                int.Parse(temp[5]),
                                                answers
                                                ));
            }
        }

        public void EditLine(int id, string title, string content)
        {
            List<String> lines = new List<String>();

            if (File.Exists(FILENAME)) 
            {
                using (StreamReader reader = new StreamReader(FILENAME))
                {
                    String line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains(";"))
                        {
                            String[] split = line.Split(';');

                            if (split[0] == Convert.ToString(id))
                            {
                                split[1] = title;
                                split[2] = content;
                                line = String.Join(";", split);
                            }
                        }

                        lines.Add(line);
                    }
                }

                using (StreamWriter writer = new StreamWriter(FILENAME, false))
                {
                    foreach (String line in lines)
                        writer.WriteLine(line);
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
            int id;
            if (Questions.Count == 0)
                id = 0;
            else
                id = Questions[Questions.Count - 1].Id + 1;

            long milisec = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            QuestionModel question = new QuestionModel(id, title, content, milisec, "N/A", 0, new List<AnswerModel>());
            Questions.Add(question);
            File.AppendAllText(FILENAME, 
                $"\n{id}," +
                $"{title}," +
                $"{content}," +
                $"{milisec}," +
                $"imglink," +
                $"vote," +
                $"{GetFormattedAnswers(question)}");
        }

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
            

            //Delete From .csv
            QuestionModel questionToDelete = null;
            string lineToDelete = "";

            foreach (QuestionModel item in Questions)
            {
                if (id.Equals(item.Id))
                {
                    questionToDelete = item;
                    
                    break;
                }
            }

            foreach (QuestionModel item in Questions)
            {
                if (id.Equals(item.Id))
                {
                    lineToDelete = Convert.ToString(item.Id);
                }
            }

            if (File.Exists(FILENAME))
            {
                string[] lines = File.ReadAllLines(FILENAME);

                using (StreamWriter sw = new StreamWriter(FILENAME, false))
                {
                    foreach (var line in lines)
                    {
                        string[] parts = line.Split(';');
                        if (parts[0] != lineToDelete)
                        {
                            sw.WriteLine(line);
                        }
                        else
                        {
                            //Deleted from csv

                            //deleted from questions
                            Questions.Remove(questionToDelete);
                        }
                    }
                }

            }
            else
            {
                //No FILENAME
            }

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
                                            int.Parse(temp[3])
                                            ));
            }
            return answers;
        }

        private String GetFormattedAnswers(QuestionModel question)
        {
            string[] props = new string[question.Answers.Count];
            for (int i = 0; i < question.Answers.Count; i++)
            {
                props[i] = question.Answers[i].ToString();
            }
            return string.Join(ANSWER_SEP, props) + "\n";
        }
    }
}
