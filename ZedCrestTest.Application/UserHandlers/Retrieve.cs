using Application.DTOS;
using Application.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ZedCrestTest.Persistence.DBContexts;

namespace Application.UserHandler
{
    public class  Retrieve
    {
        public class UserDetailsDto
        {
            public string UserName { get; set; }
            public List<DocumentDetails> Documents { get; set; }
        }
        public class Query : IRequest<UserDetailsDto>
        {        
            [Required (ErrorMessage ="EmailAddress is Required")]
            [EmailAddress (ErrorMessage ="Invalid Email Address")]   
            public string EmailAddress { get; set; }
            [Required (ErrorMessage ="Reference is Required")]   
            public string Reference { get; set; }
        }

        public class Handler : IRequestHandler<Query, UserDetailsDto>
        {
            private readonly ZedCrestContext _context;
            public Handler(ZedCrestContext context)
            {
                _context = context;
            }

            public async Task<UserDetailsDto> Handle(Query request, CancellationToken cancellationToken)
            {
                if (!await _context.Users.AnyAsync(x => x.EmailAddress == request.EmailAddress))
                {
                    throw new RestException(HttpStatusCode.NotFound, new { Email = "Email Does Not Exist" });
                }
                if (!await _context.UserDocuments.AnyAsync(x=> x.User.EmailAddress == request.EmailAddress && x.BatchDocumentReference == request.Reference))
                {
                    throw new RestException(HttpStatusCode.NotFound, new { Reference = "Reference Does Not Match Email" });
                }
                var savedUser = await _context.Users.SingleOrDefaultAsync(x => x.EmailAddress == request.EmailAddress);
                var userDocuments = await _context.UserDocuments.Where(x => x.User.EmailAddress == request.EmailAddress
                                && x.BatchDocumentReference == request.Reference).ToListAsync();

                var userdetails = new List<DocumentDetails>();
                var userdetail = new DocumentDetails();
                if (userDocuments.Count > 0 )
                {                   
                    foreach (var item in userDocuments)
                    {
                        userdetail.DocumentTitle = item.DocumentTitle;
                        userdetail.DocumentUrl = item.DocumentUrl;
                        userdetails.Add(userdetail);
                    }
                }             
                return new UserDetailsDto
                {
                    UserName = savedUser.UserName,
                    Documents = userdetails
                };
            }
        }
    }
}
