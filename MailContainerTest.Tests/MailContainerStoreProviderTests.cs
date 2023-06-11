using MailContainerTest.Data;
using MailContainerTest.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
        private Mock<IConfiguration> _mockConfiguration;

        [TestInitialize]
        public void Initialise()
        {
            _mockConfiguration = new Mock<IConfiguration>();
        }

        [DataTestMethod]
        [DataRow("Backup", typeof(BackupMailContainerDataStore))]
        [DataRow("Other", typeof(MailContainerDataStore))]
        public void MakeMailTransfer_ShouldLookupCorrectDataStore(string dataStoreType, Type desiredStore)
        {
            // Arrange
            _mockConfiguration.SetupGet(p => p["DataStoreType"]).Returns(dataStoreType);
            _provider = new MailContainerStoreProvider(_mockConfiguration.Object);

            // Act
            var dataStore = _provider.GetDataStore();

            // Assert
            Assert.IsInstanceOfType(dataStore, desiredStore);
        }
    }
}
