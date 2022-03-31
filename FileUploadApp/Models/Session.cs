using System;

namespace FileUploadApp.Models
{
    public class Session
    {
        public Session()
        {
            // create primary key upon creation
            Id = new Guid();

            // get the current Unix timestamp to track
            // user login
            Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        }

        // primary key
        public Guid Id { get; set; }

        // session timestamp
        public long Timestamp { get; set; }

        // the user that this session is associated with
        public virtual User User { get; set; }
    }
}
