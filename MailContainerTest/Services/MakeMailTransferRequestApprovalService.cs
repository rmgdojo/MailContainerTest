using MailContainerTest.Types;

namespace MailContainerTest.Services
{
    public class MakeMailTransferRequestApprovalService : IMakeMailTransferRequestApprovalService
    {
        public bool TransferIsAllowed(MailContainer mailContainer, MakeMailTransferRequest request)
        {
            return request.MailType switch
            {
                MailType.StandardLetter => mailContainer.AllowedMailType.HasFlag(AllowedMailType.StandardLetter),
                MailType.LargeLetter => mailContainer.AllowedMailType.HasFlag(AllowedMailType.LargeLetter) && mailContainer.Capacity >= request.NumberOfMailItems,
                MailType.SmallParcel => mailContainer.AllowedMailType.HasFlag(AllowedMailType.SmallParcel) && mailContainer.Status == MailContainerStatus.Operational,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
