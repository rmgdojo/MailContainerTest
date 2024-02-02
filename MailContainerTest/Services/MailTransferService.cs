using MailContainerTest.Data;
using MailContainerTest.Types;
using System.Configuration;

namespace MailContainerTest.Services
{
    public class MailTransferService : IMailTransferService
    {
        private readonly IMailContainerDataStoreFactory _mailContainerDataStoreFactory;
        private readonly ICheckMailContainerStatus _checkMailContainerStatus;
        private readonly IMailTypeChecker _mailTypeChecker;

        public MailTransferService(IMailContainerDataStoreFactory mailContainerDataStoreFactory,
                                   ICheckMailContainerStatus checkMailContainerStatus,
                                   IMailTypeChecker mailTypeChecker)
        {
            _mailContainerDataStoreFactory = mailContainerDataStoreFactory;
            _checkMailContainerStatus = checkMailContainerStatus;
            _mailTypeChecker = mailTypeChecker;
        }
        public MakeMailTransferResult MakeMailTransfer(MakeMailTransferRequest request)
        {

            var dataStoreType = ConfigurationManager.AppSettings["DataStoreType"];
            var mailContainerStoreFactory = _mailContainerDataStoreFactory.CreateMailContainerDataStore(dataStoreType);

            var sourceMailContainer = mailContainerStoreFactory.GetMailContainer(request.SourceMailContainerNumber);


            MakeMailTransferResult sourceResultCheck = _checkMailContainerStatus.CheckContainerStatus(sourceMailContainer);

            if (!sourceResultCheck.Success)
            {
                return sourceResultCheck;
            }

            sourceResultCheck = _mailTypeChecker.CheckMail(request.MailType, sourceMailContainer);


            if (sourceResultCheck.Success)
            {
                var destinationMailContainer = mailContainerStoreFactory.GetMailContainer(request.DestinationMailContainerNumber);

                MakeMailTransferResult destinationResultCheck = _checkMailContainerStatus.CheckContainerStatus(destinationMailContainer);

                if (!destinationResultCheck.Success)
                {
                    return destinationResultCheck;
                }

                sourceMailContainer.Capacity -= request.NumberOfMailItems;
                destinationMailContainer.Capacity += request.NumberOfMailItems;

                mailContainerStoreFactory.UpdateMailContainer(sourceMailContainer);
                mailContainerStoreFactory.UpdateMailContainer(destinationMailContainer);
            }

            return sourceResultCheck;
        }
    }
}
