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

        public MailTransferService(IMailContainerStoreProvider mailContainerStoreProvider)
        {
            this._mailContainerStoreProvider = mailContainerStoreProvider;
        }

        public MakeMailTransferResult MakeMailTransfer(MakeMailTransferRequest request)
        {
            var dataStoreType = ConfigurationManager.AppSettings["DataStoreType"];

            MailContainer mailContainer = null;

            var mailContainerDataStore = this._mailContainerStoreProvider.GetDataStoreForType(dataStoreType);

            mailContainer = mailContainerDataStore?.GetMailContainer(request.SourceMailContainerNumber);

            var result = new MakeMailTransferResult()
            {
                Success = true
            };

            switch (request.MailType)
            {
                case MailType.StandardLetter:
                    if (mailContainer == null)
                    {
                        result.Success = false;
                    }
                    else if (!mailContainer.AllowedMailType.HasFlag(AllowedMailType.StandardLetter))
                    {
                        result.Success = false;
                    }
                    break;

                case MailType.LargeLetter:
                    if (mailContainer == null)
                    {
                        result.Success = false;
                    }
                    else if (!mailContainer.AllowedMailType.HasFlag(AllowedMailType.LargeLetter))
                    {
                        result.Success = false;
                    }
                    else if (mailContainer.Capacity < request.NumberOfMailItems)
                    {
                        result.Success = false;
                    }
                    break;

                case MailType.SmallParcel:
                    if (mailContainer == null)
                    {
                        result.Success = false;
                    }
                    else if (!mailContainer.AllowedMailType.HasFlag(AllowedMailType.SmallParcel))
                    {
                        result.Success = false;

                    }
                    else if (mailContainer.Status != MailContainerStatus.Operational)
                    {
                        result.Success = false;
                    }
                    break;
            }

            if (result.Success)
            {
                mailContainer.Capacity -= request.NumberOfMailItems;
                mailContainerDataStore.UpdateMailContainer(mailContainer);
            }

            return result;
        }
    }
}
