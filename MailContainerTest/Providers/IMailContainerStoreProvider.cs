using MailContainerTest.Data;

namespace MailContainerTest.Providers
{
    public interface IMailContainerStoreProvider
    {
        IMailContainerDataStore GetDataStore();
    }
}