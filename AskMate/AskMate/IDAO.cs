using AskMate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskMate
{
    public interface IDAO
    {
        List<QuestionModel> GetQuestions();
        QuestionModel GetQuestion(int id);
        void NewQuestion(string content);
        public List<AnswerModel> GetAnswers(int questionId);
        void EditLine(int id, string title, string Content);
    }
}
