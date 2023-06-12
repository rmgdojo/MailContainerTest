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

            MailContainer mailContainer = GetMailContainerFromDataStoreType(request, dataStoreType);

            var result = new MakeMailTransferResult();
            result.Success = IsTheContainerInValidState(request, mailContainer);

            if (result.Success)
            {
                mailContainer.Capacity = UpdateContainerCapacity(request, mailContainer, dataStoreType);
            }

            return result;
        }

        private MailContainer GetMailContainerFromDataStoreType(MakeMailTransferRequest request, string? dataStoreType)
        {
            MailContainer mailContainer = null;

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

            return mailContainer;
        }

        private bool IsTheContainerInValidState(MakeMailTransferRequest request, MailContainer mailContainer)
        {
            if (mailContainer == null) return false;

            bool isTheContainerInValidState = true;
            switch (request.MailType)
            {
                case MailType.StandardLetter:
                    if (!mailContainer.AllowedMailType.HasFlag(AllowedMailType.StandardLetter))
                    {
                        isTheContainerInValidState = false;
                    }
                    break;

                case MailType.LargeLetter:
                    if (!mailContainer.AllowedMailType.HasFlag(AllowedMailType.LargeLetter))
                    {
                        isTheContainerInValidState = false;
                    }
                    else if (mailContainer.Capacity < request.NumberOfMailItems)
                    {
                        isTheContainerInValidState = false;
                    }
                    break;

                case MailType.SmallParcel:
                    if (!mailContainer.AllowedMailType.HasFlag(AllowedMailType.SmallParcel))
                    {
                        isTheContainerInValidState = false;
                    }
                    else if (mailContainer.Status != MailContainerStatus.Operational)
                    {
                        isTheContainerInValidState = false;
                    }
                    break;
            }

            return isTheContainerInValidState;
        }

        private int UpdateContainerCapacity(MakeMailTransferRequest request, MailContainer mailContainer, string? dataStoreType)
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

            return mailContainer.Capacity;
        }
    }
}