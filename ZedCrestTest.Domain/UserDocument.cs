using System;


namespace Domain
{
    public partial class UserDocument
    {
        public int DocumentId { get; set; }
        public string DocumentTitle { get; set; }
        public string DocumentUrl { get; set; }
        public string DocumentPublicId { get; set; }
        public DateTime DateCreated { get; set; }
        public string BatchDocumentReference { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}
