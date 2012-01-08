using System;
using System.ServiceModel;

namespace AzureClientInterface
{
    [ServiceContract]
    public interface IJobBroker
    {
        [OperationContract]
        void SubmitImageForProcessing(String clientId, Guid requestId, Uri imageLibraryUri, String originalLocation, Byte tileSize, Byte slices, byte[] imgBytes);
    }
}
