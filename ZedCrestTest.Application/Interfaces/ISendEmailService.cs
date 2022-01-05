using System.Threading.Tasks;
using Application.DTOS;

namespace Application.Interfaces
{
    public interface ISendEmailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}