using MailContainerTest.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailContainerTest.Tests.Services
{
    internal static class Helpers
    {
        internal static MailContainer GenerateValidMailContainer()
        {
            return new MailContainer()
            {
                AllowedMailType = MailType.StandardLetter,
                Capacity = 100,
                MailContainerNumber = "RMG001",
                Status = MailContainerStatus.Operational
            };
        }

        internal static MakeMailTransferRequest GenerateValidMailTransferRequest()
        {
            return new MakeMailTransferRequest()
            {
                MailType = MailType.StandardLetter,
                NumberOfMailItems = 10,
                SourceMailContainerNumber = "RMG001",
                DestinationMailContainerNumber = "RMG002",
                TransferDate = DateTime.Now
            };
        }
    }
}
