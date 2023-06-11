using Moq;
using NUnit.Framework;
using FluentAssertions;
using MailContainerTest.Data;
using MailContainerTest.Services;
using MailContainerTest.Services.Interfaces;
using MailContainerTest.Types;

namespace MailContainerTest.Tests.Services
{
    [TestFixture]
    public class MailTransferServiceTests
    {
        private Mock<IMailContainerDataStore> mailContainerDataStoreMock;
        private Mock<IMailValidator> mailTransferValidatorMock;
        private MailTransferService sut;

        [SetUp]
        public void Setup()
        {
            mailContainerDataStoreMock = new Mock<IMailContainerDataStore>();
            mailTransferValidatorMock = new Mock<IMailValidator>();
            sut = new MailTransferService(mailContainerDataStoreMock.Object, mailTransferValidatorMock.Object);
        }

        [Test]
        public void ValidLetterRequestReturnsTrue()
        {
            // Arrange
            var request = new MakeMailTransferRequest
            {
                SourceMailContainerNumber = "123",
                MailType = MailType.StandardLetter
            };

            var mailContainer = new MailContainer()
            {
                AllowedMailType = AllowedMailType.StandardLetter
            };

            mailContainerDataStoreMock.Setup(m => m.GetMailContainer(It.IsAny<string>())).Returns(mailContainer);
            mailTransferValidatorMock.Setup(m => m.IsMailValid(It.IsAny<MailContainer>(), It.IsAny<MakeMailTransferRequest>())).Returns(true);

            // Act
            var result = sut.MakeMailTransfer(request);

            // Assert
            result.Success.Should().BeTrue();
        }

        [Test]
        public void InvalidLetterRequestReturnsFalse()
        {
            // Arrange
            var request = new MakeMailTransferRequest
            {
                SourceMailContainerNumber = "123",
                MailType = MailType.StandardLetter
            };

            var mailContainer = new MailContainer()
            {
                AllowedMailType = AllowedMailType.LargeLetter
            };

            mailContainerDataStoreMock.Setup(m => m.GetMailContainer(It.IsAny<string>())).Returns(mailContainer);
            mailTransferValidatorMock.Setup(m => m.IsMailValid(It.IsAny<MailContainer>(), It.IsAny<MakeMailTransferRequest>())).Returns(false);

            // Act
            var result = sut.MakeMailTransfer(request);

            // Assert
            result.Success.Should().BeFalse();
        }

        [Test]
        public void GetMailContainerIsCalled()
        {
            // Arrange
            var request = new MakeMailTransferRequest()
            {
                SourceMailContainerNumber = "123",
                MailType = MailType.LargeLetter,
                NumberOfMailItems = 3
            };

            var mailContainer = new MailContainer()
            {
                AllowedMailType = AllowedMailType.LargeLetter,
                Capacity = 3
            };

            mailContainerDataStoreMock.Setup(m => m.GetMailContainer(It.IsAny<string>())).Returns(mailContainer);
            mailTransferValidatorMock.Setup(m => m.IsMailValid(It.IsAny<MailContainer>(), It.IsAny<MakeMailTransferRequest>())).Returns(true);

            // Act
            var result = sut.MakeMailTransfer(request);

            // Assert
            mailContainerDataStoreMock.Verify(m => m.GetMailContainer(It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void IsMailValidIsCalled()
        {
            // Arrange
            var request = new MakeMailTransferRequest()
            {
                SourceMailContainerNumber = "123",
                MailType = MailType.LargeLetter,
                NumberOfMailItems = 3
            };

            var mailContainer = new MailContainer()
            {
                AllowedMailType = AllowedMailType.LargeLetter,
                Capacity = 3
            };

            mailContainerDataStoreMock.Setup(m => m.GetMailContainer(It.IsAny<string>())).Returns(mailContainer);
            mailTransferValidatorMock.Setup(m => m.IsMailValid(It.IsAny<MailContainer>(), It.IsAny<MakeMailTransferRequest>())).Returns(true);

            // Act
            var result = sut.MakeMailTransfer(request);

            // Assert
            mailTransferValidatorMock.Verify(m => m.IsMailValid(It.IsAny<MailContainer>(), It.IsAny<MakeMailTransferRequest>()), Times.Once());
        }

        [Test]
        public void CapacityReduceWhenMailIsValid()
        {
            // Arrange
            var request = new MakeMailTransferRequest()
            {
                SourceMailContainerNumber = "123",
                MailType = MailType.LargeLetter,
                NumberOfMailItems = 2
            };

            var mailContainer = new MailContainer()
            {
                AllowedMailType = AllowedMailType.LargeLetter,
                Capacity = 3
            };

            mailContainerDataStoreMock.Setup(m => m.GetMailContainer(It.IsAny<string>())).Returns(mailContainer);
            mailTransferValidatorMock.Setup(m => m.IsMailValid(It.IsAny<MailContainer>(), It.IsAny<MakeMailTransferRequest>())).Returns(true);

            // Act
            var result = sut.MakeMailTransfer(request);

            // Assert
            mailContainer.Capacity.Should().Be(1);
        }

        [Test]
        public void CapacityNotReducedWhenMailValidationFails()
        {
            // Arrange
            var request = new MakeMailTransferRequest()
            {
                SourceMailContainerNumber = "123",
                MailType = MailType.LargeLetter,
                NumberOfMailItems = 2
            };

            var mailContainer = new MailContainer()
            {
                AllowedMailType = AllowedMailType.SmallParcel,
                Capacity = 3
            };

            mailContainerDataStoreMock.Setup(m => m.GetMailContainer(It.IsAny<string>())).Returns(mailContainer);
            mailTransferValidatorMock.Setup(m => m.IsMailValid(It.IsAny<MailContainer>(), It.IsAny<MakeMailTransferRequest>())).Returns(false);

            // Act
            var result = sut.MakeMailTransfer(request);

            // Assert
            mailContainer.Capacity.Should().Be(3);
        }
    }
}
