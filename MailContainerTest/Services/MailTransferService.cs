using MailContainerTest.Data;
using MailContainerTest.Services.Interfaces;
using MailContainerTest.Types;

namespace MailContainerTest.Services
{
    public class MailTransferService : IMailTransferService
    {
        private readonly IMailContainerDataStore mailContainerDataStore;
        private readonly IMailValidator mailValidator;
        public MailTransferService(IMailContainerDataStore mailContainerDataStore, IMailValidator mailValidator)
        {
            this.mailContainerDataStore = mailContainerDataStore;
            this.mailValidator = mailValidator;
        }

        public MakeMailTransferResult MakeMailTransfer(MakeMailTransferRequest request)
        {
            var mailContainer = mailContainerDataStore.GetMailContainer(request.SourceMailContainerNumber);
            var result = new MakeMailTransferResult()
            {
                Success = false
            };

            if (mailValidator.IsMailValid(mailContainer, request))
            {
                mailContainer.Capacity -= request.NumberOfMailItems;
                mailContainerDataStore.UpdateMailContainer(mailContainer);
                result.Success = true;
            }

            return result;
        }
    }
}
