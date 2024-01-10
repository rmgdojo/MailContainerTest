using MailContainerTest.Data;
using MailContainerTest.Services;
using MailContainerTest.Types;
using Moq;
using NUnit.Framework;

namespace MailContainerTest.Tests
{
    public class MailTransferServiceTests
    {
        [Test]
        [TestCase(MailType.StandardLetter, AllowedMailType.StandardLetter)]
        [TestCase(MailType.StandardLetter, AllowedMailType.StandardLetter | AllowedMailType.LargeLetter)]
        [TestCase(MailType.StandardLetter, AllowedMailType.StandardLetter | AllowedMailType.SmallParcel)]
        [TestCase(MailType.StandardLetter, AllowedMailType.StandardLetter | AllowedMailType.LargeLetter | AllowedMailType.SmallParcel)]
        [TestCase(MailType.LargeLetter, AllowedMailType.LargeLetter)]
        [TestCase(MailType.LargeLetter, AllowedMailType.StandardLetter | AllowedMailType.LargeLetter)]
        [TestCase(MailType.LargeLetter, AllowedMailType.LargeLetter | AllowedMailType.SmallParcel)]
        [TestCase(MailType.LargeLetter, AllowedMailType.StandardLetter | AllowedMailType.LargeLetter | AllowedMailType.SmallParcel)]
        [TestCase(MailType.SmallParcel, AllowedMailType.SmallParcel)]
        [TestCase(MailType.SmallParcel, AllowedMailType.StandardLetter | AllowedMailType.SmallParcel)]
        [TestCase(MailType.SmallParcel, AllowedMailType.LargeLetter | AllowedMailType.SmallParcel)]
        [TestCase(MailType.SmallParcel, AllowedMailType.StandardLetter | AllowedMailType.LargeLetter | AllowedMailType.SmallParcel)]
        public void MakeMailTransfer_MailTypeMatch_AllowsTransfer(MailType mailType, AllowedMailType allowedMailType)
        {
            var dataStore = new Mock<IMailContainerDataStore>();
            dataStore.Setup(m => m.GetMailContainer(It.IsAny<string>())).Returns(new MailContainer
            {
                AllowedMailType = allowedMailType,
                Capacity = 10,
            });

            var transferService = new MailTransferService(dataStore.Object);

            var request = new MakeMailTransferRequest
            {
                MailType = mailType,
                NumberOfMailItems = 1,
            };

            var result = transferService.MakeMailTransfer(request);

            Assert.That(result.Success, Is.True);

            dataStore.Verify(
                m => m.UpdateMailContainer(It.IsAny<MailContainer>()),
                Times.Once
            );
        }

        [Test]
        [TestCase(MailType.StandardLetter, AllowedMailType.LargeLetter)]
        [TestCase(MailType.StandardLetter, AllowedMailType.SmallParcel)]
        [TestCase(MailType.StandardLetter, AllowedMailType.LargeLetter | AllowedMailType.SmallParcel)]
        [TestCase(MailType.LargeLetter, AllowedMailType.StandardLetter)]
        [TestCase(MailType.LargeLetter, AllowedMailType.SmallParcel)]
        [TestCase(MailType.LargeLetter, AllowedMailType.StandardLetter | AllowedMailType.SmallParcel)]
        [TestCase(MailType.SmallParcel, AllowedMailType.StandardLetter)]
        [TestCase(MailType.SmallParcel, AllowedMailType.LargeLetter)]
        [TestCase(MailType.SmallParcel, AllowedMailType.StandardLetter | AllowedMailType.LargeLetter)]
        public void MakeMailTransfer_MailTypeMismatch_PreventsTransfer(MailType mailType, AllowedMailType allowedMailType)
        {
            var dataStore = new Mock<IMailContainerDataStore>();
            dataStore.Setup(m => m.GetMailContainer(It.IsAny<string>())).Returns(new MailContainer
            {
                AllowedMailType = allowedMailType,
                Capacity = 10,
            });

            var transferService = new MailTransferService(dataStore.Object);

            var request = new MakeMailTransferRequest
            {
                MailType = mailType,
                NumberOfMailItems = 1,
            };

            var result = transferService.MakeMailTransfer(request);

            Assert.That(result.Success, Is.False);

            dataStore.Verify(
                m => m.UpdateMailContainer(It.IsAny<MailContainer>()),
                Times.Never
            );
        }

        [Test]
        public void MakeMailTransfer_TransferAllowed_DestinationContainerIsUpdated()
        {
            var dataStore = new Mock<IMailContainerDataStore>();
            dataStore.Setup(m => m.GetMailContainer(It.IsAny<string>())).Returns(new MailContainer
            {
                AllowedMailType = AllowedMailType.StandardLetter,
                Capacity = 10,
            });

            var transferService = new MailTransferService(dataStore.Object);

            var request = new MakeMailTransferRequest
            {
                MailType = MailType.StandardLetter,
                NumberOfMailItems = 1,
            };

            var result = transferService.MakeMailTransfer(request);

            dataStore.Verify(
                m => m.UpdateMailContainer(It.IsAny<MailContainer>()),
                Times.Once
            );
        }
    }
}
