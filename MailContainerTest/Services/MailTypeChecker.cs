using MailContainerTest.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailContainerTest.Services
{
    public class MailTypeChecker : IMailTypeChecker
    {
        public MakeMailTransferResult CheckMail(MailType mailType, MailContainer mailContainer)
        {
            var result = new MakeMailTransferResult();

            if (mailContainer == null)
            {
                result.Success = false;
                return result;
            }

            switch (mailType)
            {
                case MailType.StandardLetter:

                    if (!mailContainer.AllowedMailType.Equals(AllowedMailType.StandardLetter))
                    {
                        result.Success = false;
                    }
                    else
                    {
                        result.Success = true;
                    }
                    break;

                case MailType.LargeLetter:

                    if (!mailContainer.AllowedMailType.Equals(AllowedMailType.LargeLetter))
                    {
                        result.Success = false;
                    }
                    else
                    {
                        result.Success = true;
                    }

                    break;

                case MailType.SmallParcel:

                    if (!mailContainer.AllowedMailType.Equals(AllowedMailType.SmallParcel))
                    {
                        result.Success = false;

                    }
                    else
                    {
                        result.Success = true;
                    }
                    break;

                default:
                    result.Success = false;
                    break;
            }

            return result;
        }
    }
}
