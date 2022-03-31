using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using FileUploadApp.Models;


namespace FileUploadApp
{
  public class DbUtil
  {
    // think of it as a reference to our database
    private MyDbContext dbContext;

    // the database context is dependency-injected
    // into our constructor
    public DbUtil(MyDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public void Seed()
    {
        SeedUsers();
    }

    public void SeedUsers()
    {
      // get a hash algorithm object
      HashAlgorithm sha = SHA256.Create();

      string[] usernames = { "john", "lisa" };

      // as our system's default, new users have their 
      // passwords set as "secret"
      string password = "secret";

      foreach (string username in usernames) {
        // assuming user's password is the same as username
        // we are concatenating (i.e. username + password) to generate
        // a password hash to store in the database
        string combo = username + password;
        byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(combo));

        // add a row into our User table via a User model
        dbContext.Add(new User {
            Username = username,
            PassHash = hash
        });

        // commit our changes in the database
        dbContext.SaveChanges();
      }
    }
  }
}