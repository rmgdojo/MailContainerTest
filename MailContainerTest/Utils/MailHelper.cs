using MailContainerTest.Data;
using MailContainerTest.Types;
using System.Diagnostics;

namespace MailContainerTest.Utils
{
    public static class MailHelper
    {
        /// <summary>
        /// Gets a Mail Container from the Data Store
        /// </summary>
        /// <param name="dataStoreType">Name of the Data Store type</param>
        /// <param name="containerNumber">The container number to fetch</param>
        /// <returns>The Mail Container found</returns>
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

        /// <summary>
        /// Validates the <see cref="AllowedMailType"/> of any conainer
        /// </summary>
        /// <param name="mailType">The <see cref="MailType"> to transfer</param>
        /// <param name="mailContainer">The Mail Container to validate</param>
        /// <returns>True or False</returns>
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

        /// <summary>
        /// Updates Mail Containers
        /// </summary>
        /// <param name="dataStoreType">Name of the Data Store Type</param>
        /// <param name="numberOfMailItems">Number of items to transfer</param>
        /// <param name="sourceMailContainer">The Source Container</param>
        /// <param name="destinationMailContainer">The Destination Container</param>
        /// <returns>True or False</returns>
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
