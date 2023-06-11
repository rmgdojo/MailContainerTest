using MailContainerTest.Types;

namespace MailContainerTest.Services.Interfaces
{
    public interface IMailTransferService
    {
        MakeMailTransferResult MakeMailTransfer(MakeMailTransferRequest request);
    }
}