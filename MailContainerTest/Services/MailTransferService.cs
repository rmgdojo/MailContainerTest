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


            MakeMailTransferResult result = _checkMailContainerStatus.CheckContainerStatus(sourceMailContainer);

            if (!result.Success)
            {
                return result;
            }

            result = _mailTypeChecker.CheckMail(request.MailType, sourceMailContainer);


            if (result.Success)
            {
                sourceMailContainer.Capacity -= request.NumberOfMailItems;

                if (dataStoreType == "Backup")
                {
                    var mailContainerDataStore = new BackupMailContainerDataStore();
                    mailContainerDataStore.UpdateMailContainer(sourceMailContainer);

                }
                else
                {
                    var mailContainerDataStore = new MailContainerDataStore();
                    mailContainerDataStore.UpdateMailContainer(sourceMailContainer);
                }
            }

            return result;
        }
    }
}
