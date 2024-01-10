using MailContainerTest.Data;
using MailContainerTest.Types;

namespace MailContainerTest.Services
{
    public class MailTransferService : IMailTransferService
    {
        private readonly IMailContainerDataStore _containerDataStore;
        private readonly IMakeMailTransferRequestApprovalService _requestApprovalService;

        public MailTransferService(
            IMailContainerDataStore containerDataStore, 
            IMakeMailTransferRequestApprovalService requestApprovalService) 
        {
            _containerDataStore = containerDataStore;
            _requestApprovalService = requestApprovalService;
        }

        public MakeMailTransferResult MakeMailTransfer(MakeMailTransferRequest request)
        {
            var mailContainer = _containerDataStore.GetMailContainer(request.SourceMailContainerNumber);

            var success = _requestApprovalService.TransferIsAllowed(mailContainer, request);
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
    }
}
