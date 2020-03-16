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
        void NewQuestion(string content);
        String GetAnswer(int questionId);
    }
}
