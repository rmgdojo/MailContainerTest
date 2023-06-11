using MailContainerTest.Services.Interfaces;
using MailContainerTest.Types;

namespace MailContainerTest.Services
{
    public class MailValidator : IMailValidator
    {
        public bool IsMailValid(MailContainer mailContainer, MakeMailTransferRequest request)
        {
            // Check if the mail container was instantiated
            if (mailContainer == null) return false;

            switch (request.MailType)
            {
                case MailType.StandardLetter:
                    return IsStandardLetterValid(mailContainer);

                case MailType.LargeLetter:
                    return IsLargeLetterValid(mailContainer, request.NumberOfMailItems);

                case MailType.SmallParcel:
                    return IsSmallParcelValid(mailContainer);

                default:
                    break;
            }

            return false;
        }

        private static bool IsStandardLetterValid(MailContainer mailContainer)
        {
            return mailContainer.AllowedMailType.HasFlag(AllowedMailType.StandardLetter);
        }

        private static bool IsLargeLetterValid(MailContainer mailContainer, int numberOfMailItems)
        {
            return mailContainer.AllowedMailType.HasFlag(AllowedMailType.LargeLetter) &&
                numberOfMailItems <= mailContainer.Capacity;
        }

        private static bool IsSmallParcelValid(MailContainer mailContainer)
        {
            return mailContainer.AllowedMailType.HasFlag(AllowedMailType.SmallParcel) &&
                mailContainer.Status == MailContainerStatus.Operational;
        }
    }
}
