using MailContainerTest.Data;
using MailContainerTest.Types;
using MailContainerTest.Utils;
using System.Configuration;

namespace MailContainerTest.Services
{
    public class MailTransferService : IMailTransferService
    {
        public MakeMailTransferResult MakeMailTransfer(MakeMailTransferRequest request)
        {
            var dataStoreType = ConfigurationManager.AppSettings[Constants.DATASTORE_NAME];

            //If the Data Store does not exixt, then return failed transfer result
            if(dataStoreType == null)
                return new MakeMailTransferResult { Success = false };

            /* Get the Mail Containter with the Container number from either tha backup store or main store
             * NB: The initial code was validating the Source Containter as the target, however, I have refactored this to validate 
             * the both the Source Container and the Destination containter. The only validation for the Source Container (in this case) is to check
             * if it exist
             */
            MailContainer sourceMailContainer = MailHelper.GetContainerFromContainter(Constants.BACKUP_DATASTORE_TYPE, request.SourceMailContainerNumber);

            //If there are no container or the container is not in operation, abort transfer process
            //as we cannot transfer from a container that is not in operation.
            if (sourceMailContainer == null || sourceMailContainer?.Status != MailContainerStatus.Operational)
                return new MakeMailTransferResult { Success = false };


            MailContainer destinationMailContainer = MailHelper.GetContainerFromContainter(Constants.BACKUP_DATASTORE_TYPE, request.DestinationMailContainerNumber);

            //If there are no container or the container is not in operation, abort transfer process
            if (destinationMailContainer == null || destinationMailContainer?.Status != MailContainerStatus.Operational)
                return new MakeMailTransferResult { Success = false };

            //If the mail type is not in the allowed mail type for this container, abort transfer.
            if(!MailHelper.IsAllowedMailType(request.MailType, destinationMailContainer))
                return new MakeMailTransferResult { Success = false };

            //All conditions are stisfied, we can tranfer the mails and update accordingly
            bool transferResult = MailHelper.UpdateContainerInStore(Constants.BACKUP_DATASTORE_TYPE, request.NumberOfMailItems, sourceMailContainer, destinationMailContainer);
            
            return transferResult == true ? 
                new MakeMailTransferResult { Success = true } : 
                new MakeMailTransferResult { Success = false };          
            
        }
    }
}
