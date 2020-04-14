using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskMate.Models
{
    public class UserModel
    {
        public String Email { get; private set; }
        public String Name { get; private set; }
        public String Password { get; private set; }
        public long Date { get; }
        public int Reputation { get; private set; }

        public UserModel(string email, string name, string password, long date)
        {
            Email = email;
            Name = name;
            Password = password;
            Date = date;
        }

        public UserModel(string email, string name, string password, long date, int reputation)
        {
            Email = email;
            Name = name;
            Password = password;
            Date = date;
            Reputation = reputation;
        }

        public DateTime GetDate() { return new DateTime(1970, 1, 1).AddMilliseconds(double.Parse(Date.ToString())); }
        public void IncreaseReputation(int value) { Reputation += value; }
        public void DecreaseReputation(int value) { Reputation -= value; }
    }
}
