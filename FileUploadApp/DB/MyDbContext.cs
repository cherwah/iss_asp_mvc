using System;
using Microsoft.EntityFrameworkCore;

namespace FileUploadApp.Models
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }

        // maps to a Users table in our database
        public DbSet<User> Users { get; set; }

        // maps to a Sessions table in our database
        public DbSet<Session> Sessions { get; set; }

        // maps to a Photos table in our database
        public DbSet<Photo> Photos { get; set; }
    }
}
