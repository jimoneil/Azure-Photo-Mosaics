using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace AzureClientInterface
{
    [ServiceContract]
    public interface IStorageBrokerInternal
    {
        [OperationContract]
        String GetStorageConnectionStringForClient(String clientRegistrationId);

        [OperationContract]
        String GetStorageConnectionStringForAccount(Uri accountUri);

        [OperationContract]
        ServiceBusConfiguration GetServiceBusConfiguration(String clientRegistrationId);

        [OperationContract]
        String GetStorageConnectionStringForApplication();
    }

    [DataContract]
    public class ServiceBusConfiguration
    {
        [DataMember]
        public String Namespace { get; private set; }
        [DataMember]
        public String Issuer { get; private set; }
        [DataMember]
        public String Secret { get; private set; }

        internal ServiceBusConfiguration(String n, String i, String s)
        {
            Namespace = n;
            Issuer = i;
            Secret = s;
        }
    }
}