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
        List<AnswerModel> GetAnswers(int questionId);

        void EditLine(int id, string title, string Content);
        void NewQuestion(string title, string content, int userid, List<TagModel> newTags);
        void QuestionRefresh(QuestionModel question);
        void AnswerRefresh(AnswerModel answer);
        void CommentRefresh(CommentModel comment);
        void DeleteQuestion(int id);
        void AddLinkToTable(string filePath, string table, int id);
        void LoadFiles();
    }
}
