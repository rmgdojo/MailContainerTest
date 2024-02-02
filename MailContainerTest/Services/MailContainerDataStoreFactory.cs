using MailContainerTest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailContainerTest.Services
{
    public class MailContainerDataStoreFactory : IMailContainerDataStoreFactory
    {
        public IMailContainerDataStore CreateMailContainerDataStore(string? dataStoreType)
        {
            if (dataStoreType?.Equals("Backup", StringComparison.OrdinalIgnoreCase) == true)
            {
                return new BackupMailContainerDataStore();
            }
            else
            {
                return new MailContainerDataStore();
            }
        }
    }
}
