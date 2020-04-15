using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AskMate.Models;

namespace AskMate.Services
{
    public interface IUserService
    {
        public List<UserModel> GetAll();
        public UserModel GetOne(int id);

        public UserModel GetOne(string email);

        public UserModel Login(string email, string password);
    }
}
