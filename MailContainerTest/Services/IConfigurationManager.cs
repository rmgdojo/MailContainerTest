using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailContainerTest.Services
{
    public interface IConfigurationManager
    {
        public string? AppSettings(string? key);
    }
}
