using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CloudDAL;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace AzureClientInterface
{
    public class StorageBroker : IStorageBroker
    {
        public IEnumerable<KeyValuePair<Uri, String>> EnumerateImageLibraries(String clientRegistrationId)
        {
            try
            {
                String connString = RoleEnvironment.GetConfigurationSettingValue("ApplicationStorageConnectionString");
                return new BlobAccessor(connString).GetContainerUrisAndDescriptions("ImageLibraryDescription");
            }
            catch (Exception e)
            {
                Trace.TraceError("Client: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                    clientRegistrationId,
                    e.GetType(),
                    e.Message,
                    e.StackTrace,
                    Environment.NewLine);

                throw new SystemException("Unable to retrieve list of image libraries", e);
            }
        }

        public IEnumerable<String> EnumerateImages(Uri container)
        {
            try
            {
                String connString = new StorageBrokerInternal().GetStorageConnectionStringForApplication();
                return from b in new BlobAccessor(connString).RetrieveImageUris(container)
                       select b.ToString();
            }
            catch (Exception e)
            {
                Trace.TraceError("Request: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                    default(Guid),
                    e.GetType(),
                    e.Message,
                    e.StackTrace,
                    Environment.NewLine);

                throw new SystemException("Unable to enumerate images in container", e);
            }
        }

        public ServiceBusConfiguration GetServiceBusConfiguration(String clientRegistrationId)
        {
            try
            {
                return new StorageBrokerInternal().GetServiceBusConfiguration(clientRegistrationId);
            }
            catch (Exception e)
            {
                Trace.TraceError("Client: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                    clientRegistrationId,
                    e.GetType(),
                    e.Message,
                    e.StackTrace,
                    Environment.NewLine);

                throw new SystemException("Unable to access Service Bus configuration", e);
            }
        }

        public IEnumerable<JobEntry> GetJobsForClient(String clientRegistrationId)
        {
            try
            {
                String connString = new StorageBrokerInternal().GetStorageConnectionStringForClient(clientRegistrationId);
                return new TableAccessor(connString).GetJobsForClient(clientRegistrationId);
            }
            catch (Exception e)
            {
                Trace.TraceError("Client: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                    clientRegistrationId,
                    e.GetType(),
                    e.Message,
                    e.StackTrace,
                    Environment.NewLine);

                throw new SystemException("Unable to retrieve jobs for client", e);
            }
        }

        public IEnumerable<StatusEntry> GetStatusEntriesForJob(String clientRegistrationId, Guid jobId)
        {
            try
            {
                String connString = new StorageBrokerInternal().GetStorageConnectionStringForClient(clientRegistrationId);
                return new TableAccessor(connString).GetStatusEntriesForJob(jobId);
            }
            catch (Exception e)
            {
                Trace.TraceError("Request: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                    jobId,
                    e.GetType(),
                    e.Message,
                    e.StackTrace,
                    Environment.NewLine);

                throw new SystemException(String.Format("Unable to retrieve status information for job {0}", jobId), e);
            }
        }
    }
}