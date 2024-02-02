using MailContainerTest.Services;
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
    }
}
