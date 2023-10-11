using MailContainerTest.Data;
using MailContainerTest.Types;
using System.Configuration;

namespace MailContainerTest.Services
{
    public class MailTransferService : IMailTransferService
    {
        private readonly IDataStoreConnectionService _dataStoreConnectionService;
        private readonly IMailContainerDataStore _mailContainerDataStore;

        public MailTransferService(IDataStoreConnectionService dataStoreConnectionService)
        {
            _dataStoreConnectionService = dataStoreConnectionService;
            _mailContainerDataStore = _dataStoreConnectionService.GetDataStore();
        }

        public MakeMailTransferResult MakeMailTransfer(MakeMailTransferRequest request)
        {
            if (request == null) return new();

            if (_mailContainerDataStore == null) return new();

            // Retrieve the source container
            MailContainer sourceMailContainer = _mailContainerDataStore.GetMailContainer(request.SourceMailContainerNumber);
            if (sourceMailContainer == null) return new();

            // Retrieve the destination container
            MailContainer destinationMailContainer = _mailContainerDataStore.GetMailContainer(request.DestinationMailContainerNumber);
            if (destinationMailContainer == null) return new();

            // Validate request
            if (!ValidateRequest(request, sourceMailContainer, destinationMailContainer)) return new();

            // Process request and update DataStore
            if (!ProcessRequest(request, sourceMailContainer, destinationMailContainer)) return new();

            return new() { Success = true };
        }

        private bool ValidateRequest(MakeMailTransferRequest request, MailContainer source, MailContainer destination)
        {
            if (request == null || source == null || destination == null) return false;

            if (request.NumberOfMailItems <= 0) return false;

            if (!source.AllowedMailType.HasFlag(request.MailType)) return false;
            if (!destination.AllowedMailType.HasFlag(request.MailType)) return false;

            if (destination.Capacity < request.NumberOfMailItems) return false;

            if (source.Status == MailContainerStatus.OutOfService) return false;
            if (destination.Status == MailContainerStatus.OutOfService ||
                destination.Status == MailContainerStatus.NoTransfersIn) return false;

            // There could be some additional checks here, such as the request date, depending on requirements

            return true;
        }

        private bool ProcessRequest(MakeMailTransferRequest request, MailContainer sourceContainer, MailContainer destinationContainer)
        {
            sourceContainer.Capacity -= request.NumberOfMailItems;
            destinationContainer.Capacity += request.NumberOfMailItems;

            try
            {
                _mailContainerDataStore.UpdateMailContainer(sourceContainer);
                _mailContainerDataStore.UpdateMailContainer(destinationContainer);
            }
            catch { return false; }

            return true;
        }
    }
}