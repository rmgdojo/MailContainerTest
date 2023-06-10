using MailContainerTest.Data;
using MailContainerTest.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailContainerTest.Utils
{
    public static class MailHelper
    {
        public static MailContainer GetMailFromContainter(string dataStoreType, string containerNumber)
        {
            if (dataStoreType == "Backup")
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
                MailType.StandardLetter => mailContainer.AllowedMailType.HasFlag(AllowedMailType.StandardLetter),
                MailType.LargeLetter => mailContainer.AllowedMailType.HasFlag(AllowedMailType.LargeLetter),
                MailType.SmallParcel => mailContainer.AllowedMailType.HasFlag(AllowedMailType.SmallParcel),
                _ => false
            };
        }
    }

}
