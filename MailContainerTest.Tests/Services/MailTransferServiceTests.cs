using MailContainerTest.Data;
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

namespace MailContainerTest.Tests.Services
{
    [TestClass]
    public class MailTransferServiceTests
    {
        private MailTransferService _service;
        private Mock<IMailContainerDataStore> _dataStore;
        private Mock<IDataStoreConnectionService> _containerFactory;

        string _sourceContainerId = "RMG001";
        string _destContainerId = "RMG002";

        [TestInitialize]
        public void Initialise()
        {
            _dataStore = new Mock<IMailContainerDataStore>();
            _containerFactory = new Mock<IDataStoreConnectionService>();

            _dataStore.Setup(x => x.GetMailContainer(_sourceContainerId)).Returns(Helpers.GenerateValidMailContainer());
            _dataStore.Setup(x => x.GetMailContainer(_destContainerId)).Returns(Helpers.GenerateValidMailContainer());

            _containerFactory.Setup(x => x.GetDataStore()).Returns(_dataStore.Object);

            _service = new MailTransferService(_containerFactory.Object);
        }

        [TestMethod]
        public void TransferMailValidationService_MailTransferSuccessful_ReturnsTrue()
        {
            // Arrange
            MakeMailTransferRequest request = Helpers.GenerateValidMailTransferRequest();

            // Act
            var result = _service.MakeMailTransfer(request);

            // Assert
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public void TransferMailValidationService_UpdatesMailValuesInDataStore()
        {
            // Arrange
            MakeMailTransferRequest request = Helpers.GenerateValidMailTransferRequest();

            var source = Helpers.GenerateValidMailContainer();
            var dest = Helpers.GenerateValidMailContainer();

            source.Capacity = 100;
            dest.Capacity = 50;
            request.NumberOfMailItems = 15;

            _dataStore.Setup(x => x.GetMailContainer(_sourceContainerId)).Returns(source);
            _dataStore.Setup(x => x.GetMailContainer(_destContainerId)).Returns(dest);

            // Act
            var result = _service.MakeMailTransfer(request);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(source.Capacity, 85);
            Assert.AreEqual(dest.Capacity, 65);
            _dataStore.Verify(x => x.UpdateMailContainer(source), Times.Once);
            _dataStore.Verify(x => x.UpdateMailContainer(dest), Times.Once);
        }

        [TestMethod]
        public void TransferMailValidationService_DataStoreNotFound_ResturnsFalse()
        {
            // Arrange
            MakeMailTransferRequest request = Helpers.GenerateValidMailTransferRequest();
            _containerFactory.Setup(x => x.GetDataStore()).Returns((IMailContainerDataStore)null);
            var service = new MailTransferService(_containerFactory.Object);

            // Act
            var result = service.MakeMailTransfer(request);

            // Assert
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public void TransferMailValidationService_SourceMailContainerNotFound_ResturnsFalse()
        {
            // Arrange
            MakeMailTransferRequest request = Helpers.GenerateValidMailTransferRequest();
            _dataStore.Setup(x => x.GetMailContainer(_sourceContainerId)).Returns((MailContainer)null);

            // Act
            var result = _service.MakeMailTransfer(request);

            // Assert
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public void TransferMailValidationService_DestinationMailContainerNotFound_ResturnsFalse()
        {
            // Arrange
            MakeMailTransferRequest request = Helpers.GenerateValidMailTransferRequest();
            _dataStore.Setup(x => x.GetMailContainer(_destContainerId)).Returns((MailContainer)null);

            // Act
            var result = _service.MakeMailTransfer(request);

            // Assert
            Assert.IsFalse(result.Success);
        }

        #region Process Request

        [TestMethod]
        public void TransferMailValidationService_ProcessRequestFailedWithException_ResturnsFalse()
        {
            // Arrange
            MakeMailTransferRequest request = Helpers.GenerateValidMailTransferRequest();
            _dataStore.Setup(x => x.UpdateMailContainer(It.IsAny<MailContainer>())).Throws(new ArgumentOutOfRangeException());

            // Act
            var result = _service.MakeMailTransfer(request);

            // Assert
            Assert.IsFalse(result.Success);
        }

        #endregion

        #region Validate Request

        [TestMethod]
        public void TransferMailValidationService_ValidRequest_ReturnsTrue()
        {
            // Arrange
            MakeMailTransferRequest request = Helpers.GenerateValidMailTransferRequest();
            MailContainer source = Helpers.GenerateValidMailContainer();
            MailContainer destination = Helpers.GenerateValidMailContainer();

            _dataStore.Setup(x => x.GetMailContainer(_sourceContainerId)).Returns(source);
            _dataStore.Setup(x => x.GetMailContainer(_destContainerId)).Returns(destination);

            // Act
            var result = _service.MakeMailTransfer(request);

            // Assert
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public void TransferMailValidationService_NullRequest_ReturnsFalse()
        {
            // Arrange
            MakeMailTransferRequest request = null;
            MailContainer source = Helpers.GenerateValidMailContainer();
            MailContainer destination = Helpers.GenerateValidMailContainer();

            _dataStore.Setup(x => x.GetMailContainer(_sourceContainerId)).Returns(source);
            _dataStore.Setup(x => x.GetMailContainer(_destContainerId)).Returns(destination);

            // Act
            var result = _service.MakeMailTransfer(request);

            // Assert
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public void TransferMailValidationService_NullSourceContainer_ReturnsFalse()
        {
            // Arrange
            MakeMailTransferRequest request = Helpers.GenerateValidMailTransferRequest();
            MailContainer source = null;
            MailContainer destination = Helpers.GenerateValidMailContainer();

            _dataStore.Setup(x => x.GetMailContainer(_sourceContainerId)).Returns(source);
            _dataStore.Setup(x => x.GetMailContainer(_destContainerId)).Returns(destination);

            // Act
            var result = _service.MakeMailTransfer(request);

            // Assert
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public void TransferMailValidationService_NullDestinationContainer_ReturnsFalse()
        {
            // Arrange
            MakeMailTransferRequest request = Helpers.GenerateValidMailTransferRequest();
            MailContainer source = Helpers.GenerateValidMailContainer();
            MailContainer destination = null;

            _dataStore.Setup(x => x.GetMailContainer(_sourceContainerId)).Returns(source);
            _dataStore.Setup(x => x.GetMailContainer(_destContainerId)).Returns(destination);

            // Act
            var result = _service.MakeMailTransfer(request);

            // Assert
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public void TransferMailValidationService_LowDestinationCapacity_ReturnsFalse()
        {
            // Arrange
            MakeMailTransferRequest request = Helpers.GenerateValidMailTransferRequest();
            MailContainer source = Helpers.GenerateValidMailContainer();
            MailContainer destination = Helpers.GenerateValidMailContainer();

            request.NumberOfMailItems = 100;
            destination.Capacity = 50;

            _dataStore.Setup(x => x.GetMailContainer(_sourceContainerId)).Returns(source);
            _dataStore.Setup(x => x.GetMailContainer(_destContainerId)).Returns(destination);

            // Act
            var result = _service.MakeMailTransfer(request);

            // Assert
            Assert.IsFalse(result.Success);
        }

        [DataTestMethod]
        [DataRow(MailContainerStatus.Operational, true)]
        [DataRow(MailContainerStatus.OutOfService, false)]
        [DataRow(MailContainerStatus.NoTransfersIn, false)]
        public void TransferMailValidationService_DestinationStates_ReturnsValue(MailContainerStatus status, bool expectedResult)
        {
            // Arrange
            MakeMailTransferRequest request = Helpers.GenerateValidMailTransferRequest();
            MailContainer source = Helpers.GenerateValidMailContainer();
            MailContainer destination = Helpers.GenerateValidMailContainer();

            destination.Status = status;

            _dataStore.Setup(x => x.GetMailContainer(_sourceContainerId)).Returns(source);
            _dataStore.Setup(x => x.GetMailContainer(_destContainerId)).Returns(destination);

            // Act
            var result = _service.MakeMailTransfer(request);

            // Assert
            Assert.AreEqual(result.Success, expectedResult);
        }

        [DataTestMethod]
        [DataRow(MailContainerStatus.Operational, true)]
        [DataRow(MailContainerStatus.NoTransfersIn, true)]
        [DataRow(MailContainerStatus.OutOfService, false)]
        public void TransferMailValidationService_SourceStates_ReturnsValue(MailContainerStatus status, bool expectedResult)
        {
            // Arrange
            MakeMailTransferRequest request = Helpers.GenerateValidMailTransferRequest();
            MailContainer source = Helpers.GenerateValidMailContainer();
            MailContainer destination = Helpers.GenerateValidMailContainer();

            source.Status = status;

            _dataStore.Setup(x => x.GetMailContainer(_sourceContainerId)).Returns(source);
            _dataStore.Setup(x => x.GetMailContainer(_destContainerId)).Returns(destination);

            // Act
            var result = _service.MakeMailTransfer(request);

            // Assert
            Assert.AreEqual(result.Success, expectedResult);
        }

        [TestMethod]
        public void TransferMailValidationService_ZeroItemsInRequest_ReturnsFalse()
        {
            // Arrange
            MakeMailTransferRequest request = Helpers.GenerateValidMailTransferRequest();
            MailContainer source = Helpers.GenerateValidMailContainer();
            MailContainer destination = Helpers.GenerateValidMailContainer();

            request.NumberOfMailItems = 0;

            _dataStore.Setup(x => x.GetMailContainer(_sourceContainerId)).Returns(source);
            _dataStore.Setup(x => x.GetMailContainer(_destContainerId)).Returns(destination);

            // Act
            var result = _service.MakeMailTransfer(request);

            // Assert
            Assert.IsFalse(result.Success);
        }

        [DataTestMethod]
        [DataRow(MailType.StandardLetter, MailType.StandardLetter, MailType.StandardLetter, true)]
        [DataRow(MailType.LargeLetter, MailType.LargeLetter, MailType.LargeLetter, true)]
        [DataRow(MailType.SmallParcel, MailType.SmallParcel, MailType.SmallParcel, true)]
        [DataRow(MailType.LargeLetter, MailType.SmallParcel, MailType.SmallParcel, false)]
        [DataRow(MailType.StandardLetter, MailType.LargeLetter, MailType.SmallParcel, false)]
        [DataRow(MailType.StandardLetter, MailType.SmallParcel, MailType.SmallParcel, false)]
        [DataRow(MailType.StandardLetter, MailType.StandardLetter, MailType.LargeLetter, false)]
        [DataRow(MailType.StandardLetter, MailType.StandardLetter, MailType.SmallParcel, false)]
        public void TransferMailValidationService_MailTypesCombinations_ReturnsValue(MailType requestType, MailType sourceType, MailType destType, bool expectedResult)
        {
            // Arrange
            MakeMailTransferRequest request = Helpers.GenerateValidMailTransferRequest();
            MailContainer source = Helpers.GenerateValidMailContainer();
            MailContainer destination = Helpers.GenerateValidMailContainer();

            request.MailType = requestType;
            source.AllowedMailType = sourceType;
            destination.AllowedMailType = destType;

            _dataStore.Setup(x => x.GetMailContainer(_sourceContainerId)).Returns(source);
            _dataStore.Setup(x => x.GetMailContainer(_destContainerId)).Returns(destination);

            // Act
            var result = _service.MakeMailTransfer(request);

            // Assert
            Assert.AreEqual(result.Success, expectedResult);
        }

        #endregion
    }
}
