using MailContainerTest.Data;
using MailContainerTest.Types;

namespace MailContainerTest.Services
{
    public class MailTransferService : IMailTransferService
    {
        private readonly IMailContainerDataStore _containerDataStore;

        public MailTransferService(IMailContainerDataStore containerDataStore) 
        {
            _containerDataStore = containerDataStore;
        }

        public MakeMailTransferResult MakeMailTransfer(MakeMailTransferRequest request)
        {
            var mailContainer = _containerDataStore.GetMailContainer(request.SourceMailContainerNumber);

            var success = TransferIsAllowed(mailContainer, request);
            if (success)
            {
                mailContainer.Capacity -= request.NumberOfMailItems;
                _containerDataStore.UpdateMailContainer(mailContainer);
            }

            return new MakeMailTransferResult
            {
                Success = success,
            };
        }

        private static bool TransferIsAllowed(MailContainer mailContainer, MakeMailTransferRequest request)
        {
            if (mailContainer is null)
            {
                return false;
            }

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
