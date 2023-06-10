using MailContainerTest.Data;
using MailContainerTest.Types;
using System.Diagnostics;

namespace MailContainerTest.Utils
{
    public static class MailHelper
    {
        public static MailContainer GetContainerFromContainter(string dataStoreType, string containerNumber)
        {
            if (dataStoreType == Constants.BACKUP_DATASTORE_TYPE)
            {
                var mailContainerDataStore = new BackupMailContainerDataStore();
                return mailContainerDataStore.GetMailContainer(containerNumber);

            }
            else
            {
                var mailContainerDataStore = new MailContainerDataStore();
                return mailContainerDataStore.GetMailContainer(containerNumber);
            }
        }


        public static bool IsAllowedMailType(MailType mailType, MailContainer mailContainer)
        {
            return mailType switch
            {
                MailType.StandardLetter => mailContainer.AllowedMailType.Contains(AllowedMailType.StandardLetter),
                MailType.LargeLetter => mailContainer.AllowedMailType.Contains(AllowedMailType.LargeLetter),
                MailType.SmallParcel => mailContainer.AllowedMailType.Contains(AllowedMailType.SmallParcel),
                _ => false
            };
        }

        public static bool UpdateContainerInStore(string dataStoreType, int numberOfMailItems, MailContainer sourceMailContainer, MailContainer destinationMailContainer)
        {
            try
            {
                destinationMailContainer.Capacity += numberOfMailItems;
                sourceMailContainer.Capacity -= numberOfMailItems;

                if (dataStoreType == Constants.BACKUP_DATASTORE_TYPE)
                {
                    var mailContainerDataStore = new BackupMailContainerDataStore();
                    mailContainerDataStore.UpdateMailContainer(sourceMailContainer);
                    mailContainerDataStore.UpdateMailContainer(destinationMailContainer);

                }
                else
                {
                    var mailContainerDataStore = new MailContainerDataStore();
                    mailContainerDataStore.UpdateMailContainer(sourceMailContainer);
                    mailContainerDataStore.UpdateMailContainer(destinationMailContainer);
                }

                return true;
            }
            catch (Exception ex)
            {
                //Log Error (Somewhere)
                Debug.WriteLine(ex);
            }

            return false;
        }
    }

}
