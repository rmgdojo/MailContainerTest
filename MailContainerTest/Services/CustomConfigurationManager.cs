using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailContainerTest.Services
{
    public class CustomConfigurationManager : IConfigurationManager
    {
        public string? AppSettings(string? key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
