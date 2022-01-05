using Application.UserR;
using Microsoft.AspNetCore.Http;


namespace Application.Interfaces
{
    public interface IDocumentsAccessor
    {
        DocumentUploadResult AddDocument(IFormFile file);
    }
}
