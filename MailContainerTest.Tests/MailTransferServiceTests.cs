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
        public void MakeMailTransfer_TransferAllowed_DestinationContainerIsUpdated()
        {
            var dataStore = new Mock<IMailContainerDataStore>();

            dataStore
                .Setup(m => m.GetMailContainer(It.IsAny<string>()))
                .Returns(new MailContainer());

            var requestApprovalService = new Mock<IMakeMailTransferRequestApprovalService>();

            requestApprovalService
                .Setup(m => m.TransferIsAllowed(It.IsAny<MailContainer>(), It.IsAny<MakeMailTransferRequest>()))
                .Returns(true);

            var transferService = new MailTransferService(dataStore.Object, requestApprovalService.Object);

            var result = transferService.MakeMailTransfer(new MakeMailTransferRequest());

            dataStore.Verify(
                m => m.UpdateMailContainer(It.IsAny<MailContainer>()),
                Times.Once
            );
        }

        [Test]
        public void MakeMailTransfer_TransferRejected_DestinationContainerIsNotUpdated()
        {
            var dataStore = new Mock<IMailContainerDataStore>();

            dataStore
                .Setup(m => m.GetMailContainer(It.IsAny<string>()))
                .Returns(new MailContainer());

            var requestApprovalService = new Mock<IMakeMailTransferRequestApprovalService>();

            requestApprovalService
                .Setup(m => m.TransferIsAllowed(It.IsAny<MailContainer>(), It.IsAny<MakeMailTransferRequest>()))
                .Returns(false);

            var transferService = new MailTransferService(dataStore.Object, requestApprovalService.Object);

            var result = transferService.MakeMailTransfer(new MakeMailTransferRequest());

            dataStore.Verify(
                m => m.UpdateMailContainer(It.IsAny<MailContainer>()),
                Times.Never
            );
        }
    }
}
