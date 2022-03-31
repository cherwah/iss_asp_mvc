using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FileUploadApp.Models
{
    public class Photo
    {
        public Photo()
        {
            // create primary key upon creation
            Id = new Guid();
        }

        // primary key
        public Guid Id { get; set; }

        // the photo's filename on the server
        public string Filename { get; set; }

        // points to the user that this photo belongs to
        public virtual User User { get; set; }
    }
}
