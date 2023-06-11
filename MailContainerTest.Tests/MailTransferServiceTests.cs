using MailContainerTest.Data;
using MailContainerTest.Providers;
using MailContainerTest.Services;
using MailContainerTest.Types;
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
    public class MailTransferServiceTests
    {
        private MailTransferService _service;

        private Mock<IMailContainerStoreProvider> _mockMailContainerStoreProvider;

        private Mock<IMailContainerDataStore> _mockMailContainerStore;

        [TestInitialize]
        public void Initialise()
        {
            this._mockMailContainerStoreProvider = new Mock<IMailContainerStoreProvider>();
            this._mockMailContainerStore = new Mock<IMailContainerDataStore>();
            this._service = new MailTransferService(_mockMailContainerStoreProvider.Object);
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
            this._mockMailContainerStoreProvider.Verify(x => x.GetDataStoreForType(dataStoreType), Times.Once);
        }

        [TestMethod]
        public void MakeMailTransfer_ShouldGetSourceContainer()
        {
            // Arrange
            var request = new MakeMailTransferRequest
            {
                SourceMailContainerNumber = "1",
                DestinationMailContainerNumber = "2",
                NumberOfMailItems = 1,
                TransferDate = DateTime.UtcNow,
                MailType = MailType.SmallParcel
            };

            var mockMailContainerStore = new Mock<IMailContainerDataStore>();
            _mockMailContainerStoreProvider.Setup(x => x.GetDataStoreForType(It.IsAny<string>()))
                .Returns(mockMailContainerStore.Object);

            // Act
            _service.MakeMailTransfer(request);

            // Assert
            mockMailContainerStore.Verify(x => x.GetMailContainer(request.SourceMailContainerNumber), Times.Once);
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

            var desiredContainer = new MailContainer
            {
                MailContainerNumber = "1",
                Capacity = 999,
                Status = MailContainerStatus.Operational,
                AllowedMailType = allowedMailType
            };

            this.MockMailContainerResponse(desiredContainer);

            // Act
            var result = _service.MakeMailTransfer(request);

            // Assert
            Assert.IsTrue(result.Success);
        }

        [DataTestMethod]
        [DataRow(MailType.LargeLetter, AllowedMailType.LargeLetter)]
        [DataRow(MailType.StandardLetter, AllowedMailType.StandardLetter)]
        [DataRow(MailType.SmallParcel, AllowedMailType.SmallParcel)]
        public void MakeMailTransfer_ShouldUpdateMailContainerForAllowedMailTypes(MailType requestMailType, AllowedMailType allowedMailType)
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

            var desiredContainer = new MailContainer
            {
                MailContainerNumber = "1",
                Capacity = 999,
                Status = MailContainerStatus.Operational,
                AllowedMailType = allowedMailType
            };

            this.MockMailContainerResponse(desiredContainer);

            // Act
            _service.MakeMailTransfer(request);

            // Assert
            _mockMailContainerStore.Verify(x => x.UpdateMailContainer(It.Is<MailContainer>(y => y.MailContainerNumber == desiredContainer.MailContainerNumber 
                                                                        && y.Capacity == desiredContainer.Capacity 
                                                                        && y.Status == desiredContainer.Status
                                                                        && y.AllowedMailType == desiredContainer.AllowedMailType)
                ), Times.Once);
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


        private void MockMailContainerResponse(MailContainer container)
        {
            this._mockMailContainerStore.Setup(x => x.GetMailContainer(It.IsAny<string>())).Returns(container);

            _mockMailContainerStoreProvider.Setup(x => x.GetDataStoreForType(It.IsAny<string>()))
                .Returns(this._mockMailContainerStore.Object);
        }
    }
}
