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
            var sourceMailContainer = _containerDataStore.GetMailContainer(request.SourceMailContainerNumber);

            var success = _requestApprovalService.TransferIsAllowed(sourceMailContainer, request);
            if (success)
            {
                sourceMailContainer.Capacity += request.NumberOfMailItems;

                _containerDataStore.UpdateMailContainer(sourceMailContainer);

                var destinationMailContainer = _containerDataStore.GetMailContainer(request.DestinationMailContainerNumber);
                destinationMailContainer.Capacity -= request.NumberOfMailItems;

                _containerDataStore.UpdateMailContainer(destinationMailContainer);
            }

            return new MakeMailTransferResult
            {
                Success = success,
            };
        }
    }
}
