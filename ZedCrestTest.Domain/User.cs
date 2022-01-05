using System;
using System.Collections.Generic;

#nullable disable

namespace Domain
{
    public partial class User
    {
        public User()
        {
            UserDocuments = new HashSet<UserDocument>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual ICollection<UserDocument> UserDocuments { get; set; }
    }
}
