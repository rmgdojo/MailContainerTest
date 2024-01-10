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
    }
}
