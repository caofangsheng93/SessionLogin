using LoginMVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace LoginMVC.DAL
{
    public class LoginDbContext:DbContext
    {
        public LoginDbContext()
            : base("name=DbConnectionString")
        { 
        
        }

        public DbSet<LoginModel> LoginModel { get; set; }
    }
}