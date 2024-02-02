using MailContainerTest.Services;
using Moq;
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
        //private Mock<IConfigurationManager> _configurationManager = default!;
        private Mock<ICheckMailContainerStatus> _checkMailContainer = default!;
        private MailTransferService? _subjectMailTransferService;
        private MailContainerDataStoreFactory? _subjectMailContainerDataStoreFactory;
        private CheckMailContainerStatus _subjectCheckMailContainerStatus = default!;
        private MailTypeChecker _subjectMailTypeChecker = default!;
    }
}
