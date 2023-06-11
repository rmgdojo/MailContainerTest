using FluentAssertions;
using MailContainerTest.Services;
using MailContainerTest.Services.Interfaces;
using MailContainerTest.Types;
using NUnit.Framework;

namespace MailContainerTest.Tests.Services
{
    [TestFixture]
    public class MailValidatorTests
    {
        private IMailValidator sut;


        [SetUp]
        public void Setup()
        {
            sut = new MailValidator();
        }

        [Test]
        public void ValidStandardLetterReturnsTrue()
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

            // Act
            var result = sut.IsMailValid(mailContainer, request);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void ValidLargeLetterReturnsTrue()
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

            
            // Act
            var result = sut.IsMailValid(mailContainer, request);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void ValidSmallParcellReturnsTrue()
        {
            // Arrange
            var request = new MakeMailTransferRequest()
            {
                SourceMailContainerNumber = "123",
                MailType = MailType.SmallParcel,
            };

            var mailContainer = new MailContainer()
            {
                AllowedMailType = AllowedMailType.SmallParcel,
                Status = MailContainerStatus.Operational
            };

            // Act
            var result = sut.IsMailValid(mailContainer, request);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void MailContainerNotInstantiatedReturnsFalse()
        {
            // Arrange
            var request = new MakeMailTransferRequest()
            {
                SourceMailContainerNumber = "123",
                MailType = MailType.StandardLetter
            };

            MailContainer mailContainer = null;

            // Act
            var result = sut.IsMailValid(mailContainer, request);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void InvalidStandardLetterReturnsFalse()
        {
            // Arrange
            var request = new MakeMailTransferRequest()
            {
                SourceMailContainerNumber = "123",
                MailType = MailType.StandardLetter
            };

            var mailContainer = new MailContainer()
            {
                AllowedMailType = AllowedMailType.LargeLetter
            };

            // Act
            var result = sut.IsMailValid(mailContainer, request);

            // Assert
            result.Should().BeFalse();
        }

        [TestCase(MailType.StandardLetter, 2)]
        [TestCase(MailType.LargeLetter, 1)]
        [TestCase(MailType.StandardLetter, 1)]
        public void InvalidLargeLetterReturnsFalse(AllowedMailType mailType, int capacity)
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
                AllowedMailType = mailType,
                Capacity = capacity
            };

            // Act
            var result = sut.IsMailValid(mailContainer, request);

            // Assert
            result.Should().BeFalse();
        }

        [TestCase(MailType.StandardLetter, MailContainerStatus.Operational)]
        [TestCase(MailType.SmallParcel, MailContainerStatus.OutOfService)]
        [TestCase(MailType.StandardLetter, MailContainerStatus.OutOfService)]
        public void InvalidSmallParcelReturnsFalse(AllowedMailType mailType, MailContainerStatus containerStatus)
        {
            // Arrange
            var request = new MakeMailTransferRequest()
            {
                SourceMailContainerNumber = "123",
                MailType = MailType.SmallParcel,
                NumberOfMailItems = 2
            };

            var mailContainer = new MailContainer()
            {
                AllowedMailType = mailType,
                Status = containerStatus
            };

            // Act
            var result = sut.IsMailValid(mailContainer, request);

            // Assert
            result.Should().BeFalse();
        }
    }
}