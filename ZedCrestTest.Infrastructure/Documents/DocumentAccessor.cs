using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Application.Interfaces;
using Microsoft.Extensions.Options;
using Application.UserR;
using Microsoft.AspNetCore.Http;
using System;

namespace Infrastructure.Documents
{
    public class DocumentsAccessor : IDocumentsAccessor
    {
        private readonly Cloudinary _cloudinary;
        public DocumentsAccessor(IOptions<CloudinarySettings> config)
        {
            var acc = new Account
            (
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }
        public DocumentUploadResult AddDocument(IFormFile file)
        {
            var uploadResult = new RawUploadResult();
            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new RawUploadParams
                    {
                        File = new FileDescription(file.FileName, stream)
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }
            if (uploadResult.Error != null)
                throw new Exception(uploadResult.Error.Message);

            return new DocumentUploadResult
            {
                DocUrl = uploadResult.SecureUrl.AbsoluteUri,
                Id = uploadResult.PublicId
               
            };
        }
    }
}
