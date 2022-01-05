using System.Collections.Generic;
using Microsoft.AspNetCore.Http;


namespace Application.DTOS
{
    public class MailRequest
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string UserName { get; set; }
        public List<Documents> Documents { get; set; }
    }

    public class Documents
    {
        public string DocumentName { get; set; }
        public string DocumentType { get; set; }
        public byte[] DocumentBytes { get; set; }

    }
}
