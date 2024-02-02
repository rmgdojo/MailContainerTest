﻿using MailContainerTest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailContainerTest.Services
{
    public interface IMailContainerDataStoreFactory
    {
        IMailContainerDataStore CreateMailContainerDataStore(string? dataStoreType);
    }
}
