using MailContainerTest.Types;

namespace MailContainerTest.Services
{
    public interface IMakeMailTransferRequestApprovalService
    {
        bool TransferIsAllowed(MailContainer mailContainer, MakeMailTransferRequest request);
    }
}