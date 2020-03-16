using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace AskMate
{
    public class IDAO_Impl : IDAO
    {
        const String FILENAME = "./Resources/Questions.csv"; 
        List<QuestionModel> Questions = new List<QuestionModel>();

        public IDAO_Impl()
        {
            string[] table = File.ReadAllLines(FILENAME);

            for (int i = 0; i < table.Length; i++)
            {
                string[] temp = table[i].Split(",");
                Questions.Add(new QuestionModel(int.Parse(temp[0]), temp[1], temp[2], temp[3]));
            }
        }

        public String GetAnswer(int questionId)
        {
            foreach (QuestionModel item in Questions)
            {
                if (questionId.Equals(item.Id))
                    return item.Answer;
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

        public void NewQuestion(string content)
        {
            int id = Questions[Questions.Count - 1].Id++;
            File.AppendAllText(FILENAME, $"{id},{content},link,answer");
        }
    }
}
