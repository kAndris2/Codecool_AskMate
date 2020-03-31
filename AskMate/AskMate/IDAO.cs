using AskMate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskMate
{
    interface IDAO
    {
        List<QuestionModel> GetQuestions();
        QuestionModel GetQuestionById(int id);
        AnswerModel GetAnswerByUnique(long id);
        List<AnswerModel> GetAnswers(int questionId);

        void EditLine(int id, string title, string Content);
        void NewQuestion(string title, string content);
        void QuestionRefresh(QuestionModel question);
        void AnswerRefresh(AnswerModel answer);
        void CommentRefresh(CommentModel comment);
        void DeleteQuestion(int id);
        void AddLinkToQuestion(string filePath, int id);
        void AddLinkToAnswer(string filePath, int id);
        void LoadFiles();
    }
}
