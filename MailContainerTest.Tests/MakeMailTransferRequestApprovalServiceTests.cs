using MailContainerTest.Services;
using MailContainerTest.Types;
using NUnit.Framework;

namespace MailContainerTest.Tests
{
    public class MakeMailTransferRequestApprovalServiceTests
    {
        [Test]
        [TestCase(MailType.StandardLetter, AllowedMailType.StandardLetter, true)]
        [TestCase(MailType.StandardLetter, AllowedMailType.LargeLetter, false)]
        [TestCase(MailType.StandardLetter, AllowedMailType.SmallParcel, false)]
        [TestCase(MailType.LargeLetter, AllowedMailType.LargeLetter, true)]
        [TestCase(MailType.LargeLetter, AllowedMailType.StandardLetter, false)]
        [TestCase(MailType.LargeLetter, AllowedMailType.SmallParcel, false)]
        [TestCase(MailType.SmallParcel, AllowedMailType.SmallParcel, true)]
        [TestCase(MailType.SmallParcel, AllowedMailType.StandardLetter, false)]
        [TestCase(MailType.SmallParcel, AllowedMailType.LargeLetter, false)]
        public void TransferIsAllowed_AllowsIfMailTypesMatch(MailType mailType, AllowedMailType allowedMailType, bool shouldAllow)
        {
            var approvalService = new MakeMailTransferRequestApprovalService();

            var mailContainer = new MailContainer
            {
                AllowedMailType = allowedMailType,
                Capacity = 10,
            };

            var request = new MakeMailTransferRequest
            {
                MailType = mailType,
                NumberOfMailItems = 1,
            };

            var success = approvalService.TransferIsAllowed(mailContainer, request);

            Assert.That(success, Is.EqualTo(shouldAllow));
        }

        [Test]
        [TestCase(2, 1, true)]
        [TestCase(1, 1, true)]
        [TestCase(1, 2, false)]
        public void TransferIsAllowed_LargeLetter_AllowsIfSufficientCapacity(int capacity, int numberOfItems, bool shouldAllow)
        {
            var approvalService = new MakeMailTransferRequestApprovalService();

            var mailContainer = new MailContainer
            {
                AllowedMailType = AllowedMailType.LargeLetter,
                Capacity = capacity,
            };

            var request = new MakeMailTransferRequest
            {
                MailType = MailType.LargeLetter,
                NumberOfMailItems = numberOfItems,
            };

            var success = approvalService.TransferIsAllowed(mailContainer, request);

            Assert.That(success, Is.EqualTo(shouldAllow));
        }

        [Test]
        [TestCase(MailContainerStatus.Operational, true)]
        [TestCase(MailContainerStatus.OutOfService, false)]
        [TestCase(MailContainerStatus.NoTransfersIn, false)]
        public void TransferIsAllowed_SmallParcel_AllowsIfCorrectStatus(MailContainerStatus status, bool shouldAllow)
        {
            var approvalService = new MakeMailTransferRequestApprovalService();

            var mailContainer = new MailContainer
            {
                AllowedMailType = AllowedMailType.SmallParcel,
                Capacity = 10,
                Status = status,
            };

            var request = new MakeMailTransferRequest
            {
                MailType = MailType.SmallParcel,
                NumberOfMailItems = 1,
            };

            var success = approvalService.TransferIsAllowed(mailContainer, request);

            Assert.That(success, Is.EqualTo(shouldAllow));
        }
    }
}
