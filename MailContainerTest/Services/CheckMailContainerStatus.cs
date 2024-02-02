using MailContainerTest.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailContainerTest.Services
{
    public class CheckMailContainerStatus : ICheckMailContainerStatus
    {
        public MakeMailTransferResult CheckContainerStatus(MailContainer mailContainer)
        {
            MakeMailTransferResult result = new();
            switch (mailContainer.Status)
            {
                case MailContainerStatus.Operational:
                    result.Success = true;
                    return result;
                default:
                    result.Success = false;
                    return result;
            }
        }
    }
}
