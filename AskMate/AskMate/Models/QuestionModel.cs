using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskMate
{
    public class Question
    {
        public int Id { get; set; }
        public String Content { get; set; }
        public String ImgLink { get; set; }
        public String Answer { get; set; }

        public Question(int id, string content, string link, string answer)
        {
            Id = id;
            Content = content;
            ImgLink = link;
            Answer = answer;
        }

        public void AddImage(string link)
        {
            ImgLink = link;
        }

        public void AddAnswer(String answer)
        {
            Answer = answer;
        }
    }
}
