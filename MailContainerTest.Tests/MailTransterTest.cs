using MailContainerTest.Services;
using MailContainerTest.Types;
using MailContainerTest.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics;

namespace MailContainerTest.Tests
{
    [TestClass]
    public class MailTransterTest
    {
        Mock<IMailTransferService> mailTransferService;

        public MailTransterTest()
        {
            mailTransferService = new Mock<IMailTransferService>();
        }

        MailContainer sourceContainer = new MailContainer
        {
            MailContainerNumber = "123",
            Capacity = 1000,
            Status = MailContainerStatus.Operational,
            AllowedMailType = new List<AllowedMailType> { AllowedMailType.SmallParcel }
        };

        MailContainer destinationContainer = new MailContainer
        {
            MailContainerNumber = "127",
            Capacity = 1000,
            Status = MailContainerStatus.Operational,
            AllowedMailType = new List<AllowedMailType> { AllowedMailType.SmallParcel }
        };


        [TestMethod]
        public void IsAllowedType_ReturnsTrue()
        {
            var result = MailHelper.IsAllowedMailType(MailType.SmallParcel, destinationContainer);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsAllowedType_ReturnsFalse()
        {
            var result = MailHelper.IsAllowedMailType(MailType.LargeLetter, destinationContainer);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UpdateContainer_ReturnsTrue()
        {
            var result = MailHelper.UpdateContainerInStore(Constants.BACKUP_DATASTORE_TYPE, 20, sourceContainer, destinationContainer);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GetContainer_IsNotNull()
        {
            var result = MailHelper.GetContainerFromContainter(Constants.BACKUP_DATASTORE_TYPE, "127");

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void MakeMailTransfer_ReturnsTrue()
        {
            var mtr = new MakeMailTransferRequest
            {
                SourceMailContainerNumber = "123",
                DestinationMailContainerNumber = "127",
                MailType = MailType.SmallParcel,
                NumberOfMailItems = 20,
                TransferDate = DateTime.Now,
            };

            mailTransferService.Setup(x => x.MakeMailTransfer(mtr)).Returns(new MakeMailTransferResult { Success = true});
            var result = mailTransferService.Object.MakeMailTransfer(mtr);

            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public void MakeMailTransfer_ReturnsFalse()
        {
            var mtr = new MakeMailTransferRequest
            {
                SourceMailContainerNumber = "123",
                DestinationMailContainerNumber = "127",
                MailType = MailType.SmallParcel,
                NumberOfMailItems = 20,
                TransferDate = DateTime.Now,
            };

            mailTransferService.Setup(x => x.MakeMailTransfer(mtr)).Returns(new MakeMailTransferResult { Success = false });
            var result = mailTransferService.Object.MakeMailTransfer(mtr);

            Assert.IsFalse(result.Success);
        }
    }
}
