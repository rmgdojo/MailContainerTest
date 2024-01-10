using MailContainerTest.Data;
using MailContainerTest.Types;
using System.Configuration;

namespace MailContainerTest.Services
{
    public class MailTransferService : IMailTransferService
    {
        public MakeMailTransferResult MakeMailTransfer(MakeMailTransferRequest request)
        {
            var dataStoreType = ConfigurationManager.AppSettings["DataStoreType"];

            MailContainer? mailContainer;

            // TODO: inject mail container data store in constructor
            if (dataStoreType == "Backup")
            {
                var mailContainerDataStore = new BackupMailContainerDataStore();
                mailContainer = mailContainerDataStore.GetMailContainer(request.SourceMailContainerNumber);
            } 
            else
            {
                var mailContainerDataStore = new MailContainerDataStore();
                mailContainer = mailContainerDataStore.GetMailContainer(request.SourceMailContainerNumber);
            }

            var success = TransferIsAllowed(mailContainer, request);
            if (success)
            {
                mailContainer.Capacity -= request.NumberOfMailItems;

                if (dataStoreType == "Backup")
                {
                    var mailContainerDataStore = new BackupMailContainerDataStore();
                    mailContainerDataStore.UpdateMailContainer(mailContainer);
                }
                else
                {
                    var mailContainerDataStore = new MailContainerDataStore();
                    mailContainerDataStore.UpdateMailContainer(mailContainer);
                }
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
