using MailContainerTest.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailContainerTest.Services
{
    public interface IMailTypeChecker
    {
        public MakeMailTransferResult CheckMail(MailType mailType, MailContainer mailContainer);
    }
}
