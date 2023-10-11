using MailContainerTest.Data;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailContainerTest.Services
{
    public class DataStoreConnectionService : IDataStoreConnectionService
    {
        public IMailContainerDataStore GetDataStore()
        {
            var dataStoreType = ConfigurationManager.AppSettings[Constants.DataStoreType];

            if (dataStoreType != null &&
                dataStoreType.Equals(Constants.BackupDataStore, StringComparison.OrdinalIgnoreCase))
            {
                return new BackupMailContainerDataStore();
            }

            return new MailContainerDataStore();
        }
    }
}
