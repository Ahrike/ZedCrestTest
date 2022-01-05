


using Application.DTOS;

namespace Application.Interfaces
{
    public interface IUserDocumentEmailPulisher
    {
        void PublishEmailToQueueAsync(MailRequest request);
    }
}
