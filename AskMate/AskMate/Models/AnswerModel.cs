using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskMate.Models
{
    public class AnswerModel
    {
        public int Id { get; set; }
        public int Vote { get; set; }
        public String Content { get; set; }
        public DateTime Date;

        public AnswerModel(int id, string content)
        {
            Id = id;
            Content = content;
            Date = DateTime.Now;
        }
    }
}
