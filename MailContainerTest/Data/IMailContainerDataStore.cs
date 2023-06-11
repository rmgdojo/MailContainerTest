using MailContainerTest.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailContainerTest.Data
{
    public interface IMailContainerDataStore
    {
        public MailContainer GetMailContainer(string mailContainerNumber);
        public void UpdateMailContainer(MailContainer mailContainer);
    }
}
