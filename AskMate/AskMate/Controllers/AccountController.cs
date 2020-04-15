using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AskMate.Models;
using Microsoft.AspNetCore.Identity;
using Npgsql;

namespace AskMate.Controllers
{
    public class AccountController : Controller
    {
        

        [HttpGet]
        public ViewResult Register()
        {
            return View();
        }
        public class RegisterViewModel
        {
            [Required, MaxLength(256)]
            public string Username { get; set; }
            [Required, MaxLength(256)]
            public string Email { get; set; }

            [Required, DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password), Compare(nameof(Password))]
            public string ConfirmPassword { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string sqlstr = "INSERT INTO users (username,email,pass) VALUES (@user,@email,@pass)";
                using (var conn = new NpgsqlConnection(Program.ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(sqlstr, conn))
                    {
                        cmd.Parameters.AddWithValue("user", model.Username);
                        cmd.Parameters.AddWithValue("email", model.Email);
                        cmd.Parameters.AddWithValue("pass", model.Password);
                        cmd.ExecuteNonQuery();
                    }
                }
                return View("RegSuccess");
            }
            else { return View(); }
        }
    }
}
