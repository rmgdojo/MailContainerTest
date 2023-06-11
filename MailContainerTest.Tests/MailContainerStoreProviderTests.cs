using MailContainerTest.Data;
using MailContainerTest.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailContainerTest.Tests
{
    [TestClass]
    public class MailContainerStoreProviderTests
    {
        private MailContainerStoreProvider _provider;

        [TestInitialize]
        public void Initialise()
        {
            _provider = new MailContainerStoreProvider();
        }

        [DataTestMethod]
        [DataRow("Backup", typeof(BackupMailContainerDataStore))]
        [DataRow("Other", typeof(MailContainerDataStore))]
        public void MakeMailTransfer_ShouldLookupCorrectDataStore(string dataStoreType, Type desiredStore)
        {
            // Arrange
            ConfigurationManager.AppSettings["DataStoreType"] = dataStoreType;

            // Act
            var dataStore = _provider.GetDataStoreForType(dataStoreType);

            // Assert
            Assert.IsInstanceOfType(dataStore, desiredStore);
        }
    }
}
