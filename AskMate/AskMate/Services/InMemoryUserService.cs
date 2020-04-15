using AskMate.Services;
using AskMate.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace AskMate.Services
{
    public class InMemoryUserService : IUserService
    {
        
        public IDAO_Impl Idao = IDAO_Impl.Instance;
        private List<UserModel> _users = new List<UserModel>();
        

        private bool loggedIn = false;
        public InMemoryUserService()
        {
            _users = Idao.GetUsers();
            
        }

        public List<UserModel> GetAll()
        {
            return _users;
        }

        public bool IsLoggedIn()
        {
            if (loggedIn)
            {
                return true;
            }
            else
            {
                return false;
            }
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
                loggedIn = false;
                return null;
            }
            if (user.Password != password)
            {
                loggedIn = false;
                return null;
            }
            loggedIn = true;
            return user;
        }
        
    }
}
