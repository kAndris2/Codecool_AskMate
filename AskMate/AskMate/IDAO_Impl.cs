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
                string[] temp = table[i].Split(",");
                Questions.Add(new QuestionModel(int.Parse(temp[0]), temp[1], temp[2], temp[4]));
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
                        if (line.Contains(","))
                        {
                            String[] split = line.Split(',');

                            if (split[0] == Convert.ToString(id))
                            {
                                split[1] = title;
                                split[2] = content;
                                line = String.Join(",", split);
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
            
            int id = Questions[Questions.Count - 1].Id + 1;
            File.AppendAllText(FILENAME, $"\n{id},{title},{content},date,imglink,vote,rate,answer");
            //1,Title,Content,Date,ImgLink,Vote,Rate,Answer
        }

        public void DeleteQuestion(int id)
        {
            foreach (QuestionModel item in Questions)
            {
                if (id.Equals(item.Id))
                {
                    Questions.Remove(item);
                    break;
                }
            }

            //Delete From .csv
            
            string lineToDelete = "";

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
                        string[] parts = line.Split(',');
                        if (parts[0] != lineToDelete)
                        {
                            sw.WriteLine(line);
                        }
                        else
                        {
                            //Deleted
                        }
                    }
                }
            }
            else
            {
                //No FILENAME
            }

        }
    }
}
