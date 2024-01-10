using MailContainerTest.Types;

namespace MailContainerTest.Services
{
    public class MakeMailTransferRequestApprovalService : IMakeMailTransferRequestApprovalService
    {
        public bool TransferIsAllowed(MailContainer mailContainer, MakeMailTransferRequest request)
        {
            return request.MailType switch
            {
                MailType.StandardLetter => mailContainer.AllowedMailType == AllowedMailType.StandardLetter,
                MailType.LargeLetter => mailContainer.AllowedMailType == AllowedMailType.LargeLetter && mailContainer.Capacity >= request.NumberOfMailItems,
                MailType.SmallParcel => mailContainer.AllowedMailType == AllowedMailType.SmallParcel && mailContainer.Status == MailContainerStatus.Operational,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
