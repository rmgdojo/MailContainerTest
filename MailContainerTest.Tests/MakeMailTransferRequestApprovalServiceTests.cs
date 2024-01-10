using MailContainerTest.Services;
using MailContainerTest.Types;
using NUnit.Framework;

namespace MailContainerTest.Tests
{
    public class MakeMailTransferRequestApprovalServiceTests
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
        public void TransferIsAllowed_MailTypeMatch_AllowsTransfer(MailType mailType, AllowedMailType allowedMailType)
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

            Assert.That(success, Is.True);
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
        public void TransferIsAllowed_MailTypeMismatch_PreventsTransfer(MailType mailType, AllowedMailType allowedMailType)
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

            Assert.That(success, Is.False);
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
