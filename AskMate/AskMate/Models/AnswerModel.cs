﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AskMate;

namespace AskMate.Models
{
    public class AnswerModel
    {
        public int Id { get; }
        public int Vote { get; private set; }
        public int Question_Id { get; private set; }
        public String Content { get; private set; }
        public String ImgLink { get; private set; }
        public long Date { get; }

        public AnswerModel(int id, string content, long date, int vote, int qid)
        {
            Id = id;
            Content = content;
            Date = date;
            Vote = vote;
            ImgLink = @"\Upload\Images\5d7252f7-c2f6-4a7c-9b66-37dca3acafe5.png";   //HARDCODED
            Question_Id = qid;
        }

        public DateTime GetDate() { return new DateTime(1970, 1, 1).AddMilliseconds(double.Parse(Date.ToString())); }
        public void AddImage(string link) { ImgLink = link; Refr(); }
        public void VoteUp() { Vote++; Refr(); }
        public void VoteDown() { Vote--; Refr(); }

        public long GetUnique()
        {
            return Convert.ToInt64($"{Id}{Date}");
        }

        private void Refr()
        {
            IDAO_Impl.Instance.Refresh();
        }

        public override string ToString()
        {
            return $"{Id}.{Content}.{Date}.{Vote}.{Question_Id}";
        }
    }
}
