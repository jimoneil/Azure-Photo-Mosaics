using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace AzureClientInterface
{
    [ServiceContract]
    public interface IStorageBroker
    {
        [OperationContract]
        IEnumerable<KeyValuePair<Uri, String>> EnumerateImageLibraries(String clientRegistrationId);

        [OperationContract]
        IEnumerable<String> EnumerateImages(Uri container);

        [OperationContract]
        ServiceBusConfiguration GetServiceBusConfiguration(String clientRegistrationId);

        [OperationContract]
        IEnumerable<CloudDAL.JobEntry> GetJobsForClient(String clientRegistrationId);

        [OperationContract]
        IEnumerable<CloudDAL.StatusEntry> GetStatusEntriesForJob(String clientRegistrationId, Guid jobId);
    }
}
