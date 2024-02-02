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
            var result = _subjectMailTransferService?.MakeMailTransfer(new Types.MakeMailTransferRequest());

            //assert
            return result?.Success;
        }
    }
}
