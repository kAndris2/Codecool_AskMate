using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskMate
{
    public interface IDAO
    {
        List<Question> GetQuestions();
        Question GetQuestion(int id);
        int NewQuestion(string title, string content);
        List<String> GetAnswers(int questionId);
    }
}
