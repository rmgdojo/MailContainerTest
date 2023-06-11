using MailContainerTest.Services;
using MailContainerTest.Types;
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
    public class MailTransferServiceTests
    {
        private MailTransferService _service;

        [TestInitialize]
        public void Initialise()
        {
            this._service = new MailTransferService();
        }

        [DataTestMethod]
        [DataRow("Backup")]
        [DataRow("Other")]
        public void MakeMailTransfer_ShouldLookupCorrectDataStore(string dataStoreType)
        {
            // Arrange
            ConfigurationManager.AppSettings["DataStoreType"] = dataStoreType;

            var request = new MakeMailTransferRequest
            {
                SourceMailContainerNumber = "1",
                DestinationMailContainerNumber = "2",
                NumberOfMailItems = 1,
                TransferDate = DateTime.UtcNow,
                MailType = MailType.SmallParcel
            };

            // Act
            _service.MakeMailTransfer(request);

            // Assert
            // unable to assert correct data store at present - refactoring required
        }

        [DataTestMethod]
        [DataRow(MailType.LargeLetter, AllowedMailType.LargeLetter)]
        [DataRow(MailType.StandardLetter, AllowedMailType.StandardLetter)]
        [DataRow(MailType.SmallParcel, AllowedMailType.SmallParcel)]
        public void MakeMailTransfer_ShouldAllowTransferToAllowedMailTypes(MailType requestMailType, AllowedMailType allowedMailType)
        {
            // Arrange
            var request = new MakeMailTransferRequest
            {
                SourceMailContainerNumber = "1",
                DestinationMailContainerNumber = "2",
                NumberOfMailItems = 1,
                TransferDate = DateTime.UtcNow,
                MailType = requestMailType
            };

            // need to mock mail container 
            // container needs to be operational
            // container needs to have capacity
            // container must accept correct mail type

            // Act
            var result = _service.MakeMailTransfer(request);

            // Assert
            Assert.IsFalse(result.Success);
        }

        [DataTestMethod]
        [DataRow(MailType.LargeLetter)]
        [DataRow(MailType.StandardLetter)]
        [DataRow(MailType.SmallParcel)]
        public void MakeMailTransfer_ShouldNotAllowTransferToNonOperationalMailContainers(MailType requestMailType)
        {
            // Arrange
            var request = new MakeMailTransferRequest
            {
                SourceMailContainerNumber = "1",
                DestinationMailContainerNumber = "2",
                NumberOfMailItems = 1,
                TransferDate = DateTime.UtcNow,
                MailType = requestMailType
            };

            // need to mock mail container 
            // container needs to be non operational
            // container needs to have capacity
            // container must accept correct mail type

            // Act
            var result = _service.MakeMailTransfer(request);

            // Assert
            Assert.IsFalse(result.Success);
        }

        [DataTestMethod]
        [DataRow(MailType.LargeLetter)]
        [DataRow(MailType.StandardLetter)]
        [DataRow(MailType.SmallParcel)]
        public void MakeMailTransfer_ShouldNotAllowTransferToMailContainersWithoutCapacity(MailType requestMailType)
        {
            // Arrange
            var request = new MakeMailTransferRequest
            {
                SourceMailContainerNumber = "1",
                DestinationMailContainerNumber = "2",
                NumberOfMailItems = 1,
                TransferDate = DateTime.UtcNow,
                MailType = requestMailType
            };

            // need to mock mail container 
            // container needs to be operational
            // container needs to NOT have capacity
            // container must accept correct mail type

            // Act
            var result = _service.MakeMailTransfer(request);

            // Assert
            Assert.IsFalse(result.Success);
        }

        [DataTestMethod]
        [DataRow(MailType.LargeLetter)]
        [DataRow(MailType.StandardLetter)]
        [DataRow(MailType.SmallParcel)]
        public void MakeMailTransfer_ShouldUpdateMailContainer(MailType requestMailType)
        {
            // Arrange
            var request = new MakeMailTransferRequest
            {
                SourceMailContainerNumber = "1",
                DestinationMailContainerNumber = "2",
                NumberOfMailItems = 1,
                TransferDate = DateTime.UtcNow,
                MailType = requestMailType
            };

            // need to mock mail container 
            // container needs to be operational
            // container needs to NOT have capacity
            // container must accept correct mail type

            // Act
            var result = _service.MakeMailTransfer(request);

            // Assert
            
            // cannot verify at present
        }
    }
}
