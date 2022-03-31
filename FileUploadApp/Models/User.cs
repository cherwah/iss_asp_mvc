using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FileUploadApp.Models
{
    public class User
    {
        public User()
        {
            // create primary key upon creation
            Id = new Guid();

            // instantiate our List so that we can start
            // adding Photo objects to it
            Photos = new List<Photo>();
        }

        // primary key
        public Guid Id { get; set; }

        [Required]
        public string Username { get; set; }

        // we don't store the actual password of the user in our
        // database; only store a hash of the password
        [Required]
        public byte[] PassHash { get; set; }

        // points to the list of photos that the user owns
        public virtual ICollection<Photo> Photos { get; set; }
    }
}
