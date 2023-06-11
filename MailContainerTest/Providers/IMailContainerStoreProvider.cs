using MailContainerTest.Data;

namespace MailContainerTest.Providers
{
    public interface IMailContainerStoreProvider
    {
        IMailContainerDataStore GetDataStoreForType(string type);
    }
}