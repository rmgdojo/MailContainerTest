using MailContainerTest.Data;
using MailContainerTest.Services;
using MailContainerTest.Types;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailContainerTest.Tests
{
    public class MailTransferService_Tests
    {
        private Mock<IMailContainerDataStoreFactory> _mailContainerDataStoreFactory = default!;
        private Mock<IMailTypeChecker> _mailTypeChecker = default!;
        private Mock<IMailContainerDataStore> _mailContainerDataStore = default!;
        private Mock<IConfigurationManager> _configurationManager = default!;
        private Mock<ICheckMailContainerStatus> _checkMailContainer = default!;
        private MailTransferService? _subjectMailTransferService;
        private MailContainerDataStoreFactory? _subjectMailContainerDataStoreFactory;
        private CheckMailContainerStatus _subjectCheckMailContainerStatus = default!;
        private MailTypeChecker _subjectMailTypeChecker = default!;

        [SetUp]
        public void SetUp()
        {
            _mailContainerDataStoreFactory = new Mock<IMailContainerDataStoreFactory>();
            _mailTypeChecker = new Mock<IMailTypeChecker>();
            _configurationManager = new Mock<IConfigurationManager>();
            _mailContainerDataStore = new Mock<IMailContainerDataStore>();
            _checkMailContainer = new Mock<ICheckMailContainerStatus>();

            _subjectCheckMailContainerStatus = new CheckMailContainerStatus();
            _subjectMailTypeChecker = new MailTypeChecker();
        }

        [Test]
        public void MailContainerDataStoreFactory_WhenDataStoreTypeIsbackup_ReturnBackUpMailDataStore()
        {
            _subjectMailContainerDataStoreFactory = new MailContainerDataStoreFactory();
            var factory = _subjectMailContainerDataStoreFactory.CreateMailContainerDataStore("backup");
            Assert.That(factory, Is.InstanceOf<BackupMailContainerDataStore>());
        }

        [Test]
        public void MailContainerDataStoreFactory_WhenDataStoreTypeIsNotBackup_ReturnMailContainerDataStore()
        {
            _subjectMailContainerDataStoreFactory = new MailContainerDataStoreFactory();

            var factory = _subjectMailContainerDataStoreFactory.CreateMailContainerDataStore("main");

            Assert.That(factory, Is.InstanceOf<MailContainerDataStore>());
        }

        [TestCase("backup", true, ExpectedResult = true)]
        [TestCase("whatever", false, ExpectedResult = false)]
        public bool? MakeMailTransfer_WithBackupDataStoreType_EnsureSuccess(string dataStoreType, bool resultStatus)
        {
            //arrange

            _configurationManager.Setup(x => x.AppSettings(It.IsAny<string>())).Returns(dataStoreType);

            _mailContainerDataStoreFactory.Setup(x => x.CreateMailContainerDataStore(It.IsAny<string>())).Returns(new BackupMailContainerDataStore());

            _mailContainerDataStore.Setup(x => x.GetMailContainer(It.IsAny<string>())).Returns(new MailContainer());

            _checkMailContainer.Setup(c => c.CheckContainerStatus(It.IsAny<MailContainer>())).Returns(new MakeMailTransferResult { Success = resultStatus });

            _mailTypeChecker.Setup(x => x.CheckMail(MailType.StandardLetter, It.IsAny<MailContainer>())).Returns(new MakeMailTransferResult { Success = resultStatus });

            _subjectMailTransferService = new MailTransferService(_mailContainerDataStoreFactory.Object,  _checkMailContainer.Object, _mailTypeChecker.Object, _configurationManager.Object);

            //act
            var result = _subjectMailTransferService.MakeMailTransfer(new Types.MakeMailTransferRequest());

            //assert
            return result?.Success;
        }


        [TestCase(MailContainerStatus.OutOfService, ExpectedResult = false)]
        [TestCase(MailContainerStatus.NoTransfersIn, ExpectedResult = false)]
        [TestCase(MailContainerStatus.Operational, ExpectedResult = true)]
        public bool CheckContainerStatus_IfStatusIsNotOperational_ReturnFalse(MailContainerStatus mailContainerStatus)
        {
            //arrange


            //act
            var response = _subjectCheckMailContainerStatus.CheckContainerStatus(new MailContainer() { Status = mailContainerStatus });

            //assert
            return response.Success;
        }

        [Test]
        public void CheckMailType_IfMailContainerIsNull_ReturnFalse()
        {
            //arrange

            //act
            var response = _subjectMailTypeChecker.CheckMail(MailType.LargeLetter, null);

            //assert
            Assert.That(response.Success, Is.False);
        }

        [TestCase(MailType.LargeLetter, AllowedMailType.LargeLetter, ExpectedResult = true)]
        [TestCase(MailType.LargeLetter, AllowedMailType.SmallParcel, ExpectedResult = false)]
        [TestCase(MailType.SmallParcel, AllowedMailType.SmallParcel, ExpectedResult = true)]
        [TestCase(MailType.StandardLetter, AllowedMailType.LargeLetter, ExpectedResult = false)]
        [TestCase(MailType.StandardLetter, AllowedMailType.LargeLetter, ExpectedResult = false)]
        [TestCase(MailType.StandardLetter, AllowedMailType.StandardLetter, ExpectedResult = true)]
        public bool CheckMailType_IfContainerContainsSameTypeOfMail_ReturnValue(MailType mailType, AllowedMailType allowedMailType)
        {
            //arrange

            //act
            var response = _subjectMailTypeChecker.CheckMail(mailType, new MailContainer { AllowedMailType = allowedMailType });

            //assert
            return response.Success;
        }

        [Test]
        public void MakeMailTransfer_WhenSuccessful_ChangeCapacityCountForSourceAndDestinationMailContainer()
        {
            //arrange

            var sourceCapacity = int.MaxValue;
            var destinationCapacity = 0;


            _configurationManager.Setup(x => x.AppSettings(It.IsAny<string>())).Returns("dataStoreType");

            _mailContainerDataStoreFactory.Setup(x => x.CreateMailContainerDataStore(It.IsAny<string>())).Returns(_mailContainerDataStore.Object);

            _mailContainerDataStore.Setup(x => x.GetMailContainer("source")).Returns(new MailContainer { Capacity = sourceCapacity });

            _mailContainerDataStore.Setup(x => x.GetMailContainer("destination")).Returns(new MailContainer { Capacity = destinationCapacity });

            _checkMailContainer.Setup(c => c.CheckContainerStatus(It.IsAny<MailContainer>())).Returns(new MakeMailTransferResult { Success = true });

            _mailTypeChecker.Setup(x => x.CheckMail(MailType.StandardLetter, It.IsAny<MailContainer>())).Returns(new MakeMailTransferResult { Success = true });

            var request = new Types.MakeMailTransferRequest { NumberOfMailItems = 5, SourceMailContainerNumber = "source", DestinationMailContainerNumber = "destination" };

            _subjectMailTransferService = new MailTransferService(_mailContainerDataStoreFactory.Object, _checkMailContainer.Object, _mailTypeChecker.Object, _configurationManager.Object);

            //act
            var result = _subjectMailTransferService?.MakeMailTransfer(request);

            //assert
            var sourceMailContainerCapacity = _mailContainerDataStore.Object.GetMailContainer("source").Capacity;
            var destinationMailContainerCapacity = _mailContainerDataStore.Object.GetMailContainer("destination").Capacity;

            Assert.That(sourceMailContainerCapacity, Is.EqualTo(sourceCapacity - request.NumberOfMailItems));
            Assert.That(destinationMailContainerCapacity, Is.EqualTo(destinationCapacity + request.NumberOfMailItems));
        }

        [Test]
        public void MakeMailTransfer_WhenSuccessful_InvokeUpdateMailContainer()
        {
            //arrange

            var sourceCapacity = 1000;
            var destinationCapacity = 0;


            _configurationManager.Setup(x => x.AppSettings(It.IsAny<string>())).Returns("dataStoreType");

            _mailContainerDataStoreFactory.Setup(x => x.CreateMailContainerDataStore(It.IsAny<string>())).Returns(_mailContainerDataStore.Object);

            _mailContainerDataStore.Setup(x => x.GetMailContainer("source")).Returns(new MailContainer { Capacity = sourceCapacity });

            _mailContainerDataStore.Setup(x => x.GetMailContainer("destination")).Returns(new MailContainer { Capacity = destinationCapacity });

            _checkMailContainer.Setup(c => c.CheckContainerStatus(It.IsAny<MailContainer>())).Returns(new MakeMailTransferResult { Success = true });

            _mailTypeChecker.Setup(x => x.CheckMail(MailType.StandardLetter, It.IsAny<MailContainer>())).Returns(new MakeMailTransferResult { Success = true });

            var request = new Types.MakeMailTransferRequest { NumberOfMailItems = 5, SourceMailContainerNumber = "source", DestinationMailContainerNumber = "destination" };

            _subjectMailTransferService = new MailTransferService(_mailContainerDataStoreFactory.Object, _checkMailContainer.Object, _mailTypeChecker.Object, _configurationManager.Object);

            //act
            var result = _subjectMailTransferService?.MakeMailTransfer(request);

            //assert
            var sourceMailContainerCapacity = _mailContainerDataStore.Object.GetMailContainer("source");
            var destinationMailContainerCapacity = _mailContainerDataStore.Object.GetMailContainer("destination");

            _mailContainerDataStore.Verify(x => x.UpdateMailContainer(sourceMailContainerCapacity), times: Times.Once);
            _mailContainerDataStore.Verify(x => x.UpdateMailContainer(destinationMailContainerCapacity), Times.Once);
        }

    }
}
