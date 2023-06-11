using MailContainerTest.Types;

namespace MailContainerTest.Services.Interfaces
{
    public interface IMailValidator
    {
        bool IsMailValid(MailContainer mailContainer, MakeMailTransferRequest request);
    }
}
