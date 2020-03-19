using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskMate.Models
{
    public class AnswerModel
    {
        public int Id { get; }
        public int Vote { get; private set; }
        public String Content { get; private set; }
        public long Date { get; }

        public AnswerModel(int id, string content, long date, int vote)
        {
            Id = id;
            Content = content;
            Date = date;
            Vote = vote;
        }

        public DateTime GetDate() { return new DateTime(1970, 1, 1).AddMilliseconds(double.Parse(Date.ToString())); }

        public override string ToString()
        {
            return $"{Id}.{Content}.{Date}.{Vote}";
        }

        public long GetUnique()
        {
            return Convert.ToInt64($"{Id}{Date}");
        }
    }
}
