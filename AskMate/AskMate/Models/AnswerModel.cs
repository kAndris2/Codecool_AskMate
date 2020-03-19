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
        public String ImgLink { get; private set; }

        public AnswerModel(int id, string content, long date, int vote)
        {
            Id = id;
            Content = content;
            Date = date;
            Vote = vote;
            ImgLink = @"\Upload\Images\5d7252f7-c2f6-4a7c-9b66-37dca3acafe5.png";   //HARDCODED
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

        public void AddImage(string link)
        {
            ImgLink = link; Refr();
        }

        private void Refr()
        {
            IDAO_Impl.Instance.Refresh();
        }
    }
}
