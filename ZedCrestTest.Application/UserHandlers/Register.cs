using Api.CustomAttributes;
using Application.DTOS;
using Application.Errors;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZedCrestTest.Persistence.DBContexts;

namespace Application.UserHandler
{
    public class Register
    {
        public class Command : IRequest<RegisterUserReturnDto>
        {
            [Required (ErrorMessage ="UserName is Required")]
            public string UserName { get; set; }
            [Required (ErrorMessage ="EmailAddress is Required")]
            [EmailAddress (ErrorMessage ="Invalid Email Address")]
            public string EmailAddress { get; set; }

            [MaxFileSize(2 * 1024 * 1024)]
            public IFormFileCollection File { get; set; }
        }

        public class Handler: IRequestHandler<Command, RegisterUserReturnDto>
        {
            private readonly ZedCrestContext _context;
            private readonly IDocumentsAccessor _documentsAccessor;
            private readonly IUserDocumentEmailPulisher _userDocumentEmailPublisher;
            public Handler(ZedCrestContext context, IDocumentsAccessor documentsAccessor, IUserDocumentEmailPulisher userDocumentEmailSender)
            {
                _context = context;
                _documentsAccessor = documentsAccessor;
                _userDocumentEmailPublisher = userDocumentEmailSender;
            }          
            public async Task<RegisterUserReturnDto> Handle(Command request, CancellationToken cancellationToken)
            {
                User user = new();
                /* 
                 * checking if user exist already
               */
                if (!(await _context.Users.AnyAsync(x => x.EmailAddress == request.EmailAddress) || await _context.Users.AnyAsync(x => x.UserName == request.UserName)))
                {
                     user = new User()
                    {
                        UserName = request.UserName,
                        EmailAddress = request.EmailAddress,
                        DateCreated = DateTime.Now
                    };
                    //Add User
                    _context.Users.Add(user);
                }
                else{
                     user = await _context.Users.SingleOrDefaultAsync(x => x.EmailAddress == request.EmailAddress);
                     if(user == null) user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == request.UserName);
                }

                /* Notes
  * subaccount codes can also be store in a db if the platform has many creators  
  * the subaccount code can be fetch with query or procedure with conditions that differentiate each creator(maybe a creator id )
  */

                //Upload Document to Cloudinary
                string batchReference = GetRandomNumber(15);

                AddDocumentToCloudinary(request, user, batchReference);
                
                    var success = await _context.SaveChangesAsync() > 0;
                    if (!success) throw new Exception("An error occured while saving changes");              
                var returnObj = new RegisterUserReturnDto();

                SendToQueue(request);

                returnObj.UniqueRef = batchReference;
                returnObj.Message = "Your Registration is Successful. Kindly Check your Email For Documents";
                return returnObj;

                throw new Exception("An error occured while saving changes");
            }

            private void AddDocumentToCloudinary(Command request, User user, string reference)
            {
                foreach (var file in request.File)
                {
                    var documentUploadResult = _documentsAccessor.AddDocument(file);
                    var document = new UserDocument
                    {
                        DocumentUrl = documentUploadResult.DocUrl,
                        DocumentPublicId = documentUploadResult.Id,
                        DateCreated = DateTime.Now,
                        DocumentTitle = file.FileName,
                        BatchDocumentReference = reference,
                        User = user
                        
                    };
                    /* Notes
   * subaccount codes can also be store in a db if the platform has many creators  
   * the subaccount code can be fetch with query or procedure with conditions that differentiate each creator(maybe a creator id )
   */
                    _context.UserDocuments.Add(document);
                }
            }

            private void SendToQueue(Command request)
            {
                //drop it in the queue
                MailRequest mailRequest = new();              
                List<Documents> files = new();
                foreach (var file in request.File)
                {
                    long length = file.Length;
                    byte[] fileBytes;
                    if (length < 0){
                        throw new RestException(HttpStatusCode.BadRequest, new { File = "File is Corrupt" });
                    }
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        Documents documents = new(){
                            DocumentBytes = fileBytes,
                            DocumentName = Path.GetFileNameWithoutExtension(file.FileName),
                            DocumentType = file.ContentType
                        };
                    files.Add(documents);
                }             
                mailRequest.Documents = files;
                mailRequest.ToEmail = request.EmailAddress;
                _userDocumentEmailPublisher.PublishEmailToQueueAsync(mailRequest);
            }
            public string GetRandomNumber(int length)
            {
                var rndDigits = new StringBuilder().Insert(0, "0123456789", length).ToString().ToCharArray();
                return string.Join("", rndDigits.OrderBy(o => Guid.NewGuid()).Take(length));
            }
        }
    }
}
