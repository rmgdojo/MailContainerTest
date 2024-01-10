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
                .Returns(new MailContainer
                {
                    AllowedMailType = AllowedMailType.StandardLetter,
                    Capacity = 10,
                });

            var requestApprovalService = new Mock<IMakeMailTransferRequestApprovalService>();

            requestApprovalService
                .Setup(m => m.TransferIsAllowed(It.IsAny<MailContainer>(), It.IsAny<MakeMailTransferRequest>()))
                .Returns(true);

            var transferService = new MailTransferService(dataStore.Object, requestApprovalService.Object);

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
