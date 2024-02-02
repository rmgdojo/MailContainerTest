using MailContainerTest.Data;
using MailContainerTest.Types;
using System.Configuration;

namespace MailContainerTest.Services
{
    public class MailTransferService : IMailTransferService
    {
        private readonly IMailContainerDataStoreFactory _mailContainerDataStoreFactory;

        public MailTransferService(IMailContainerDataStoreFactory mailContainerDataStoreFactory)
        {
            _mailContainerDataStoreFactory = mailContainerDataStoreFactory;
        }
        public MakeMailTransferResult MakeMailTransfer(MakeMailTransferRequest request)
        {

            var dataStoreType = ConfigurationManager.AppSettings["DataStoreType"];
            var mailContainerStoreFactory = _mailContainerDataStoreFactory.CreateMailContainerDataStore(dataStoreType);

            var sourceMailContainer = mailContainerStoreFactory.GetMailContainer(request.SourceMailContainerNumber);

            if (dataStoreType == "Backup")
            {
                var mailContainerDataStore = new BackupMailContainerDataStore();
                mailContainer = mailContainerDataStore.GetMailContainer(request.SourceMailContainerNumber);

            } else
            {
                var mailContainerDataStore = new MailContainerDataStore();
                sourceMailContainer = mailContainerDataStore.GetMailContainer(request.SourceMailContainerNumber);
            }

            var result = new MakeMailTransferResult();

            switch (request.MailType)
            {
                case MailType.StandardLetter:
                    if (sourceMailContainer == null)
                    {
                        result.Success = false;
                    }
                    else if (!sourceMailContainer.AllowedMailType.HasFlag(AllowedMailType.StandardLetter))
                    {
                        result.Success = false;
                    }
                    break;

                case MailType.LargeLetter:
                    if (sourceMailContainer == null)
                    {
                        result.Success = false;
                    }
                    else if (!sourceMailContainer.AllowedMailType.HasFlag(AllowedMailType.LargeLetter))
                    {
                        result.Success = false;
                    }
                    else if (sourceMailContainer.Capacity < request.NumberOfMailItems)
                    {
                        result.Success = false;
                    }
                    break;

                case MailType.SmallParcel:
                    if (sourceMailContainer == null)
                    {
                        result.Success = false;
                    }
                    else if (!sourceMailContainer.AllowedMailType.HasFlag(AllowedMailType.SmallParcel))
                    {
                        result.Success = false;

                    }
                    else if (sourceMailContainer.Status != MailContainerStatus.Operational)
                    {
                        result.Success = false;
                    }
                    break;
            }

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
