using System;
using System.Runtime.Serialization;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace AzureClientInterface
{
    public class StorageBrokerInternal : IStorageBrokerInternal
    {
        public String GetStorageConnectionStringForClient(String clientRegistrationId)
        {
            return String.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}",
                RoleEnvironment.GetConfigurationSettingValue("UserStorageAccountName"),
                this.GetStorageAccountKey("User"));
        }

        public String GetStorageConnectionStringForApplication()
        {
            return RoleEnvironment.GetConfigurationSettingValue("ApplicationStorageConnectionString");
        }

        public String GetStorageConnectionStringForAccount(Uri accountUri)
        {
            String account = accountUri.Host.Substring(0, accountUri.Host.IndexOf('.'));
            if (account.Equals(RoleEnvironment.GetConfigurationSettingValue("UserStorageAccountName"), StringComparison.OrdinalIgnoreCase))
                return String.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}",
                    RoleEnvironment.GetConfigurationSettingValue("UserStorageAccountName"),
                    this.GetStorageAccountKey("User"));
            else
                return GetStorageConnectionStringForApplication();
        }

        public ServiceBusConfiguration GetServiceBusConfiguration(String clientRegistrationId)
        {
            return new ServiceBusConfiguration(
                RoleEnvironment.GetConfigurationSettingValue("AppFabricNamespace"),
                RoleEnvironment.GetConfigurationSettingValue("ServiceBusIssuer"),
                RoleEnvironment.GetConfigurationSettingValue("ServiceBusSecret"));
        }

        private String GetStorageAccountKey(String account)
        {
            return RoleEnvironment.GetConfigurationSettingValue(String.Format("{0}StorageAccountKey", account));
        }
    }
}