using MailContainerTest.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailContainerTest.Providers
{
    public class MailContainerStoreProvider : IMailContainerStoreProvider
    {
        private string _dataStoreType;

        public MailContainerStoreProvider(IConfiguration configuration)
        {
            _dataStoreType = configuration["DataStoreType"];
        }

        public IMailContainerDataStore GetDataStore()
        {
            if (_dataStoreType == "Backup")
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
