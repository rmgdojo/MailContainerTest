using MailContainerTest.Data;
using MailContainerTest.Providers;
using MailContainerTest.Types;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace MailContainerTest.Services
{
    public class MailTransferService : IMailTransferService
    {
        private IMailContainerStoreProvider _mailContainerStoreProvider;
        private IMailContainerDataStore _mailContainerDataStore;

        public MailTransferService(IMailContainerStoreProvider mailContainerStoreProvider)
        {
            _mailContainerStoreProvider = mailContainerStoreProvider;

            _mailContainerDataStore = _mailContainerStoreProvider.GetDataStore();
        }

        public MakeMailTransferResult MakeMailTransfer(MakeMailTransferRequest request)
        {
            var result = new MakeMailTransferResult();

            try
            {
                var sourceMailContainer = _mailContainerDataStore?.GetMailContainer(request.SourceMailContainerNumber);

                var destinationMailContainer = _mailContainerDataStore?.GetMailContainer(request.DestinationMailContainerNumber);

                if (sourceMailContainer != null
                    && destinationMailContainer != null
                    && sourceMailContainer.AllowedMailType.HasFlag(request.MailType)
                    && CanPerformTransferBetweenContainers(sourceMailContainer, destinationMailContainer))
                {
                    var transferSucessful = PerformTransfer(request.NumberOfMailItems, sourceMailContainer, destinationMailContainer);

                    result.Success = transferSucessful;
                }
            }
            catch (Exception ex)
            {
                // Logging
            }

            return result;
        }

        private bool CanPerformTransferBetweenContainers(MailContainer sourceContainer, MailContainer destinationContainer)
        {
            bool canTransfer = true;

            if (sourceContainer.Status != MailContainerStatus.Operational || destinationContainer.Status != MailContainerStatus.Operational)
            {
                canTransfer = false;
            }

            if (sourceContainer.AllowedMailType != destinationContainer.AllowedMailType)
            {
                canTransfer = false;
            }

            return canTransfer;
        }

        private bool PerformTransfer(int numberOfItems, MailContainer sourceContainer, MailContainer destinationContainer)
        {
            sourceContainer.Capacity -= numberOfItems;
            destinationContainer.Capacity += numberOfItems;

            _mailContainerDataStore.UpdateMailContainer(sourceContainer);
            _mailContainerDataStore.UpdateMailContainer(destinationContainer);

            return true;
        }
    }
}
