using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskMate.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public String Email { get; set; }
        public String Name { get; set; }
        public String Password { get; set; }
        public long Date { get; }
        public int Reputation { get; private set; }

        public UserModel(int id, string email, string name, string password, long date)
        {
            Id = id;
            Email = email;
            Name = name;
            Password = password;
            Date = date;
        }

        public UserModel(int id, string email, string name, string password, long date, int reputation)
        {
            Id = id;
            Email = email;
            Name = name;
            Password = password;
            Date = date;
            Reputation = reputation;
        }

        public DateTime GetDate() { return new DateTime(1970, 1, 1).AddMilliseconds(double.Parse(Date.ToString())); }
        public void IncreaseReputation(int value) { Reputation += value; }
        public void DecreaseReputation(int value) { Reputation -= value; }

        public void SetMail(string mail) { Email = mail; }
        public void SetName(string name) { Name = name; }
        public void SetPass(string pass) { Password = pass; }
    }
}
