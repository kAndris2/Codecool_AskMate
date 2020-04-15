using AskMate.Services;
using AskMate.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskMate.Services
{
    public class InMemoryUserService : IUserService
    {
        private List<UserModel> _users = new List<UserModel>();

        public InMemoryUserService()
        {
            _users.Add(new UserModel(1,"asd","kaka","asd",101020));
        }

        public List<UserModel> GetAll()
        {
            return _users;
        }

        public UserModel GetOne(int id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }
        public UserModel GetOne(string email)
        {
            return _users.FirstOrDefault(u => u.Email == email);
        }

        public UserModel Login(string email, string password)
        {
            var user = GetOne(email);
            if (user == null)
            {
                return null;
            }
            if (user.Password != password)
            {
                return null;
            }
            return user;
        }
    }
}
