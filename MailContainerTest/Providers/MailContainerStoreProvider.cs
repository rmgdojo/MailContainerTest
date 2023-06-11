using MailContainerTest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailContainerTest.Providers
{
    public class MailContainerStoreProvider : IMailContainerStoreProvider
    {
        public IMailContainerDataStore GetDataStoreForType(string type)
        {
            if (type == "DataStoreType")
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
