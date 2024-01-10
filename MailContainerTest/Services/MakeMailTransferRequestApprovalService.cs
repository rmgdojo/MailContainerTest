using MailContainerTest.Types;

namespace MailContainerTest.Services
{
    public class MakeMailTransferRequestApprovalService : IMakeMailTransferRequestApprovalService
    {
        public bool TransferIsAllowed(MailContainer mailContainer, MakeMailTransferRequest request)
        {
            if (mailContainer.Capacity < request.NumberOfMailItems)
            {
                return false;
            }

            if (mailContainer.Status != MailContainerStatus.Operational)
            {
                return false;
            }

            return request.MailType switch
            {
                MailType.StandardLetter => mailContainer.AllowedMailType == AllowedMailType.StandardLetter,
                MailType.LargeLetter => mailContainer.AllowedMailType == AllowedMailType.LargeLetter,
                MailType.SmallParcel => mailContainer.AllowedMailType == AllowedMailType.SmallParcel,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
