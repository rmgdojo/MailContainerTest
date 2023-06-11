using MailContainerTest.Data;
using MailContainerTest.Providers;
using MailContainerTest.Services;
using MailContainerTest.Types;
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
    public class MailTransferServiceTests
    {
        private MailTransferService _service;

        private Mock<IMailContainerStoreProvider> _mockMailContainerStoreProvider;

        private Mock<IMailContainerDataStore> _mockMailContainerStore;

        [TestInitialize]
        public void Initialise()
        {
            _mockMailContainerStoreProvider = new Mock<IMailContainerStoreProvider>();
            _mockMailContainerStore = new Mock<IMailContainerDataStore>();

            _mockMailContainerStoreProvider.Setup(x => x.GetDataStore())
                .Returns(_mockMailContainerStore.Object);

            _service = new MailTransferService(_mockMailContainerStoreProvider.Object);
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

            // Act
            _service.MakeMailTransfer(request);

            // Assert
            _mockMailContainerStore.Verify(x => x.GetMailContainer(request.SourceMailContainerNumber), Times.Once);
        }

        [TestMethod]
        public void MakeMailTransfer_ShouldGetDestinationContainer()
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

            // Act
            _service.MakeMailTransfer(request);

            // Assert
            _mockMailContainerStore.Verify(x => x.GetMailContainer(request.DestinationMailContainerNumber), Times.Once);
        }

        [DataTestMethod]
        [DataRow(MailType.LargeLetter)]
        [DataRow(MailType.StandardLetter)]
        [DataRow(MailType.SmallParcel)]
        public void MakeMailTransfer_Valid_ShouldAllowTransfer(MailType mailtype)
        {
            // Arrange
            var request = new MakeMailTransferRequest
            {
                SourceMailContainerNumber = "1",
                DestinationMailContainerNumber = "2",
                NumberOfMailItems = 1,
                TransferDate = DateTime.UtcNow,
                MailType = mailtype
            };

            var sourceContainer = new MailContainer
            {
                MailContainerNumber = request.SourceMailContainerNumber,
                Capacity = 1,
                Status = MailContainerStatus.Operational,
                AllowedMailType = mailtype
            };

            var destinationContainer = new MailContainer
            {
                MailContainerNumber = request.DestinationMailContainerNumber,
                Capacity = 1,
                Status = MailContainerStatus.Operational,
                AllowedMailType = mailtype
            };

            MockMailContainerResponseForContainerNumber(sourceContainer);
            MockMailContainerResponseForContainerNumber(destinationContainer);

            // Act
            var result = _service.MakeMailTransfer(request);

            // Assert
            Assert.IsTrue(result.Success);
        }

        [DataTestMethod]
        [DataRow(MailType.LargeLetter)]
        [DataRow(MailType.StandardLetter)]
        [DataRow(MailType.SmallParcel)]
        public void MakeMailTransfer_Valid_ShouldUpdateSourceMailContainer(MailType mailType)
        {
            // Arrange
            var request = new MakeMailTransferRequest
            {
                SourceMailContainerNumber = "1",
                DestinationMailContainerNumber = "2",
                NumberOfMailItems = 1,
                TransferDate = DateTime.UtcNow,
                MailType = mailType
            };

            var sourceContainer = new MailContainer
            {
                MailContainerNumber = request.SourceMailContainerNumber,
                Capacity = 999,
                Status = MailContainerStatus.Operational,
                AllowedMailType = mailType
            };

            var destinationContainer = new MailContainer
            {
                MailContainerNumber = request.DestinationMailContainerNumber,
                Capacity = 999,
                Status = MailContainerStatus.Operational,
                AllowedMailType = mailType
            };

            MockMailContainerResponseForContainerNumber(sourceContainer);
            MockMailContainerResponseForContainerNumber(destinationContainer);

            // Act
            _service.MakeMailTransfer(request);

            // Assert
            _mockMailContainerStore.Verify(x => x.UpdateMailContainer(sourceContainer), Times.Once);
        }

        [DataTestMethod]
        [DataRow(MailType.LargeLetter)]
        [DataRow(MailType.StandardLetter)]
        [DataRow(MailType.SmallParcel)]
        public void MakeMailTransfer_Valid_ShouldUpdateDestinationMailContainer(MailType mailType)
        {
            // Arrange
            var request = new MakeMailTransferRequest
            {
                SourceMailContainerNumber = "1",
                DestinationMailContainerNumber = "2",
                NumberOfMailItems = 1,
                TransferDate = DateTime.UtcNow,
                MailType = mailType
            };

            var sourceContainer = new MailContainer
            {
                MailContainerNumber = request.SourceMailContainerNumber,
                Capacity = 999,
                Status = MailContainerStatus.Operational,
                AllowedMailType = mailType
            };

            var destinationContainer = new MailContainer
            {
                MailContainerNumber = request.DestinationMailContainerNumber,
                Capacity = 999,
                Status = MailContainerStatus.Operational,
                AllowedMailType = mailType
            };

            MockMailContainerResponseForContainerNumber(sourceContainer);
            MockMailContainerResponseForContainerNumber(destinationContainer);

            // Act
            _service.MakeMailTransfer(request);

            // Assert
            _mockMailContainerStore.Verify(x => x.UpdateMailContainer(destinationContainer), Times.Once);
        }

        [DataTestMethod]
        [DataRow(MailType.LargeLetter)]
        [DataRow(MailType.StandardLetter)]
        [DataRow(MailType.SmallParcel)]
        public void MakeMailTransfer_Valid_ShouldDecrementSourceContainer(MailType requestMailType)
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

            var sourceContainerQty = new Random().Next();
            var destinationContainerQty = new Random().Next();

            var sourceContainer = new MailContainer
            {
                MailContainerNumber = request.SourceMailContainerNumber,
                Capacity = sourceContainerQty,
                Status = MailContainerStatus.Operational,
                AllowedMailType = requestMailType
            };

            var destinationContainer = new MailContainer
            {
                MailContainerNumber = request.DestinationMailContainerNumber,
                Capacity = destinationContainerQty,
                Status = MailContainerStatus.Operational,
                AllowedMailType = requestMailType
            };

            MockMailContainerResponseForContainerNumber(sourceContainer);
            MockMailContainerResponseForContainerNumber(destinationContainer);

            // Act
            var result = _service.MakeMailTransfer(request);

            // Assert
            _mockMailContainerStore.Verify(x => x.UpdateMailContainer(It.Is<MailContainer>(y => y.Status == sourceContainer.Status
               && y.MailContainerNumber == sourceContainer.MailContainerNumber
               && y.AllowedMailType == sourceContainer.AllowedMailType
               && y.Capacity == (sourceContainerQty - request.NumberOfMailItems)))
                , Times.Once);
        }

        [DataTestMethod]
        [DataRow(MailType.LargeLetter)]
        [DataRow(MailType.StandardLetter)]
        [DataRow(MailType.SmallParcel)]
        public void MakeMailTransfer_Valid_ShouldIncrementDestinationContainer(MailType requestMailType)
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

            var sourceContainerQty = new Random().Next();
            var destinationContainerQty = new Random().Next();

            var sourceContainer = new MailContainer
            {
                MailContainerNumber = request.SourceMailContainerNumber,
                Capacity = sourceContainerQty,
                Status = MailContainerStatus.Operational,
                AllowedMailType = requestMailType
            };

            var destinationContainer = new MailContainer
            {
                MailContainerNumber = request.DestinationMailContainerNumber,
                Capacity = destinationContainerQty,
                Status = MailContainerStatus.Operational,
                AllowedMailType = requestMailType
            };

            MockMailContainerResponseForContainerNumber(sourceContainer);
            MockMailContainerResponseForContainerNumber(destinationContainer);

            // Act
            var result = _service.MakeMailTransfer(request);

            // Assert
            _mockMailContainerStore.Verify(x => x.UpdateMailContainer(It.Is<MailContainer>(y => y.Status == destinationContainer.Status
               && y.MailContainerNumber == destinationContainer.MailContainerNumber
               && y.AllowedMailType == destinationContainer.AllowedMailType
               && y.Capacity == (destinationContainerQty + request.NumberOfMailItems)))
                , Times.Once);
        }

        [DataTestMethod]
        [DataRow(MailType.LargeLetter, MailType.StandardLetter)]
        [DataRow(MailType.LargeLetter, MailType.SmallParcel)]
        [DataRow(MailType.StandardLetter, MailType.LargeLetter)]
        [DataRow(MailType.StandardLetter, MailType.SmallParcel)]
        [DataRow(MailType.SmallParcel, MailType.LargeLetter)]
        [DataRow(MailType.SmallParcel, MailType.StandardLetter)]
        public void MakeMailTransfer_Invalid_TransferBetweenMailTypes_ShouldReturnFalse(MailType sourceMailtype, MailType destinationMailType)
        {
            // Arrange
            var request = new MakeMailTransferRequest
            {
                SourceMailContainerNumber = "1",
                DestinationMailContainerNumber = "2",
                NumberOfMailItems = 1,
                TransferDate = DateTime.UtcNow,
                MailType = sourceMailtype
            };

            var sourceContainer = new MailContainer
            {
                MailContainerNumber = request.SourceMailContainerNumber,
                Capacity = 1,
                Status = MailContainerStatus.Operational,
                AllowedMailType = sourceMailtype
            };

            var destinationContainer = new MailContainer
            {
                MailContainerNumber = request.DestinationMailContainerNumber,
                Capacity = 1,
                Status = MailContainerStatus.Operational,
                AllowedMailType = destinationMailType
            };

            MockMailContainerResponseForContainerNumber(sourceContainer);
            MockMailContainerResponseForContainerNumber(destinationContainer);

            // Act
            var result = _service.MakeMailTransfer(request);

            // Assert
            Assert.IsFalse(result.Success);
        }

        [DataTestMethod]
        [DataRow(MailType.LargeLetter, MailType.StandardLetter)]
        [DataRow(MailType.LargeLetter, MailType.SmallParcel)]
        [DataRow(MailType.StandardLetter, MailType.LargeLetter)]
        [DataRow(MailType.StandardLetter, MailType.SmallParcel)]
        [DataRow(MailType.SmallParcel, MailType.LargeLetter)]
        [DataRow(MailType.SmallParcel, MailType.StandardLetter)]
        public void MakeMailTransfer_Invalid_TransferBetweenMailTypes_ShouldNotUpdateContainers(MailType sourceMailtype, MailType destinationMailType)
        {
            // Arrange
            var request = new MakeMailTransferRequest
            {
                SourceMailContainerNumber = "1",
                DestinationMailContainerNumber = "2",
                NumberOfMailItems = 1,
                TransferDate = DateTime.UtcNow,
                MailType = sourceMailtype
            };

            var sourceContainer = new MailContainer
            {
                MailContainerNumber = request.SourceMailContainerNumber,
                Capacity = 1,
                Status = MailContainerStatus.Operational,
                AllowedMailType = sourceMailtype
            };

            var destinationContainer = new MailContainer
            {
                MailContainerNumber = request.DestinationMailContainerNumber,
                Capacity = 1,
                Status = MailContainerStatus.Operational,
                AllowedMailType = destinationMailType
            };

            MockMailContainerResponseForContainerNumber(sourceContainer);
            MockMailContainerResponseForContainerNumber(destinationContainer);

            // Act
            _service.MakeMailTransfer(request);

            // Assert
            _mockMailContainerStore.Verify(x => x.UpdateMailContainer(It.IsAny<MailContainer>()), Times.Never);
        }

        [DataTestMethod]
        [DataRow(MailContainerStatus.NoTransfersIn)]
        [DataRow(MailContainerStatus.OutOfService)]
        public void MakeMailTransfer_Invalid_SourceContainerNotOperational_ShouldReturnFalse(MailContainerStatus containerStatus)
        {
            // Arrange
            var request = new MakeMailTransferRequest
            {
                SourceMailContainerNumber = "1",
                DestinationMailContainerNumber = "2",
                NumberOfMailItems = 1,
                TransferDate = DateTime.UtcNow,
                MailType = MailType.StandardLetter
            };

            var sourceContainer = new MailContainer
            {
                MailContainerNumber = request.SourceMailContainerNumber,
                Capacity = 1,
                Status = containerStatus,
                AllowedMailType = MailType.StandardLetter
            };

            var destinationContainer = new MailContainer
            {
                MailContainerNumber = request.DestinationMailContainerNumber,
                Capacity = 1,
                Status = MailContainerStatus.Operational,
                AllowedMailType = MailType.StandardLetter
            };

            MockMailContainerResponseForContainerNumber(sourceContainer);
            MockMailContainerResponseForContainerNumber(destinationContainer);

            // Act
            var result = _service.MakeMailTransfer(request);

            // Assert
            Assert.IsFalse(result.Success);
        }

        [DataTestMethod]
        [DataRow(MailContainerStatus.NoTransfersIn)]
        [DataRow(MailContainerStatus.OutOfService)]
        public void MakeMailTransfer_Invalid_SourceContainerNotOperational_ShouldNotUpdateContainers(MailContainerStatus containerStatus)
        {
            // Arrange
            var request = new MakeMailTransferRequest
            {
                SourceMailContainerNumber = "1",
                DestinationMailContainerNumber = "2",
                NumberOfMailItems = 1,
                TransferDate = DateTime.UtcNow,
                MailType = MailType.StandardLetter
            };

            var sourceContainer = new MailContainer
            {
                MailContainerNumber = request.SourceMailContainerNumber,
                Capacity = 1,
                Status = containerStatus,
                AllowedMailType = MailType.StandardLetter
            };

            var destinationContainer = new MailContainer
            {
                MailContainerNumber = request.DestinationMailContainerNumber,
                Capacity = 1,
                Status = MailContainerStatus.Operational,
                AllowedMailType = MailType.StandardLetter
            };

            MockMailContainerResponseForContainerNumber(sourceContainer);
            MockMailContainerResponseForContainerNumber(destinationContainer);

            // Act
            var result = _service.MakeMailTransfer(request);

            // Assert
            _mockMailContainerStore.Verify(x => x.UpdateMailContainer(It.IsAny<MailContainer>()), Times.Never);
        }

        [DataTestMethod]
        [DataRow(MailContainerStatus.NoTransfersIn)]
        [DataRow(MailContainerStatus.OutOfService)]
        public void MakeMailTransfer_Invalid_DestinationContainerNotOperational_ShouldReturnFalse(MailContainerStatus containerStatus)
        {
            // Arrange
            var request = new MakeMailTransferRequest
            {
                SourceMailContainerNumber = "1",
                DestinationMailContainerNumber = "2",
                NumberOfMailItems = 1,
                TransferDate = DateTime.UtcNow,
                MailType = MailType.StandardLetter
            };

            var sourceContainer = new MailContainer
            {
                MailContainerNumber = request.SourceMailContainerNumber,
                Capacity = 1,
                Status = MailContainerStatus.Operational,
                AllowedMailType = MailType.StandardLetter
            };

            var destinationContainer = new MailContainer
            {
                MailContainerNumber = request.DestinationMailContainerNumber,
                Capacity = 1,
                Status = containerStatus,
                AllowedMailType = MailType.StandardLetter
            };

            MockMailContainerResponseForContainerNumber(sourceContainer);
            MockMailContainerResponseForContainerNumber(destinationContainer);

            // Act
            var result = _service.MakeMailTransfer(request);

            // Assert
            Assert.IsFalse(result.Success);
        }

        [DataTestMethod]
        [DataRow(MailContainerStatus.NoTransfersIn)]
        [DataRow(MailContainerStatus.OutOfService)]
        public void MakeMailTransfer_Invalid_DestinationContainerNotOperational_ShouldNotUpdateContainers(MailContainerStatus containerStatus)
        {
            // Arrange
            var request = new MakeMailTransferRequest
            {
                SourceMailContainerNumber = "1",
                DestinationMailContainerNumber = "2",
                NumberOfMailItems = 1,
                TransferDate = DateTime.UtcNow,
                MailType = MailType.StandardLetter
            };

            var sourceContainer = new MailContainer
            {
                MailContainerNumber = request.SourceMailContainerNumber,
                Capacity = 1,
                Status = MailContainerStatus.Operational,
                AllowedMailType = MailType.StandardLetter
            };

            var destinationContainer = new MailContainer
            {
                MailContainerNumber = request.DestinationMailContainerNumber,
                Capacity = 1,
                Status = containerStatus,
                AllowedMailType = MailType.StandardLetter
            };

            MockMailContainerResponseForContainerNumber(sourceContainer);
            MockMailContainerResponseForContainerNumber(destinationContainer);

            // Act
            var result = _service.MakeMailTransfer(request);

            // Assert
            _mockMailContainerStore.Verify(x => x.UpdateMailContainer(It.IsAny<MailContainer>()), Times.Never);
        }



        private void MockMailContainerResponseForContainerNumber(MailContainer container)
        {
            _mockMailContainerStore.Setup(x => x.GetMailContainer(container.MailContainerNumber)).Returns(container);
        }
    }
}
