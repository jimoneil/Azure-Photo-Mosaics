using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace InternalStorageBrokerServices
{
    public interface IStorageBrokerChannel : AzureClientInterface.IStorageBrokerInternal, IClientChannel { }

    public static class StorageBroker
    {
        private static ChannelFactory<IStorageBrokerChannel> GetChannelFactory()
        {
            ChannelFactory<IStorageBrokerChannel> factory = null;

            try
            {
                RoleInstance selectedInstance = null;
                Byte retries = 5;
                Int32 retryInterval = 1;  // in minutes

                // there's a (slight) chance the AzureClientInterface role is not yet running/or restarting when request arrives
                // so add some retry logic with a back off up to about an hour (2+4+8+16+32 minutes)
                while ((selectedInstance == null) && (retries > 0))
                {
                    // get list of instances for the AzureClientInterface role (should return exactly one element, a collection)
                    var candidateInstances = (from r in RoleEnvironment.Roles
                                              where r.Key.Equals("AzureClientInterface", StringComparison.OrdinalIgnoreCase)
                                              select r.Value.Instances).FirstOrDefault();

                    // pick a random instance of the AzureClientInterfaceRole (poor man's load balancing)
                    selectedInstance = candidateInstances
                        .Skip(new Random(Environment.TickCount).Next(candidateInstances.Count))
                        .FirstOrDefault();

                    // no luck yet
                    if (selectedInstance == null) System.Threading.Thread.Sleep(retryInterval * 60000);
                    retries--;
                    retryInterval *= 2;
                }

                if (selectedInstance != null)
                {
                    var internalEndpoint = selectedInstance.InstanceEndpoints["InternalHttpEndpoint"];
                    factory = new ChannelFactory<IStorageBrokerChannel>(
                                  internalEndpoint.Protocol == "tcp" ? (Binding)(new NetTcpBinding()) : (Binding)(new BasicHttpBinding()),
                                  new EndpointAddress(String.Format("{0}://{1}/StorageBrokerInternal.svc",
                                        internalEndpoint.Protocol == "tcp" ? "net.tcp" : internalEndpoint.Protocol, internalEndpoint.IPEndpoint)));
                }
                else
                {
                    Trace.TraceError("Request: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                       default(Guid),
                       "N/A",
                       "No instance of AzureClientInterface available to service request",
                       "N/A",
                       Environment.NewLine);
                    throw new SystemException("No instance of AzureClientInterface available to service request");
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Request: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                    default(Guid),
                    e.GetType(),
                    e.Message,
                    e.StackTrace,
                    Environment.NewLine);
                throw;
            }

            return factory;
        }

        public static String GetStorageConnectionStringForClient(String clientId)
        {
            String connString = String.Empty;
            ChannelFactory<IStorageBrokerChannel> channelFactory = null;
            IStorageBrokerChannel channel = null;

            try
            {
                channelFactory = GetChannelFactory();
                channel = channelFactory.CreateChannel();

                channel.Open();

                connString = channel.GetStorageConnectionStringForClient(clientId);
            }
            catch (Exception e)
            {
                throw new SystemException(
                    String.Format("Unable to retrieve storage connection string for client: {0}", clientId), e);
            }
            finally
            {
                if ((channel != null) && (channel.State == CommunicationState.Opened)) channel.Close();
                if ((channelFactory != null) && (channelFactory.State == CommunicationState.Opened)) channelFactory.Close();
            }

            return connString;
        }

        public static String GetStorageConnectionStringForAccount(Uri account)
        {
            String connString = String.Empty;
            ChannelFactory<IStorageBrokerChannel> channelFactory = null;
            IStorageBrokerChannel channel = null;

            try
            {
                channelFactory = GetChannelFactory();
                channel = channelFactory.CreateChannel();

                channel.Open();

                connString = channel.GetStorageConnectionStringForAccount(account);
            }
            catch (Exception e)
            {
                throw new SystemException(
                      String.Format("Unable to retrieve storage connection string for account: {0}", account.ToString()), e);
            }
            finally
            {
                if ((channel != null) && (channel.State == CommunicationState.Opened)) channel.Close();
                if ((channelFactory != null) && (channelFactory.State == CommunicationState.Opened)) channelFactory.Close();
            }

            return connString;
        }

        public static AzureClientInterface.ServiceBusConfiguration GetServiceBusConfiguration(String clientId)
        {
            AzureClientInterface.ServiceBusConfiguration config = null;
            ChannelFactory<IStorageBrokerChannel> channelFactory = null;
            IStorageBrokerChannel channel = null;

            try
            {
                channelFactory = GetChannelFactory();
                channel = channelFactory.CreateChannel();

                channel.Open();

                config = channel.GetServiceBusConfiguration(clientId);
            }
            catch (Exception e)
            {
                throw new SystemException(
                      String.Format("Unable to retrieve service bus configuration for client: {0}", clientId), e);
            }
            finally
            {
                if ((channel != null) && (channel.State == CommunicationState.Opened)) channel.Close();
                if ((channelFactory != null) && (channelFactory.State == CommunicationState.Opened)) channelFactory.Close();
            }

            return config;
        }

        public static String GetStorageConnectionStringForApplication()
        {
            String config = null;
            ChannelFactory<IStorageBrokerChannel> channelFactory = null;
            IStorageBrokerChannel channel = null;

            try
            {
                channelFactory = GetChannelFactory();
                channel = channelFactory.CreateChannel();

                channel.Open();

                config = channel.GetStorageConnectionStringForApplication();
            }
            catch (Exception e)
            {
                throw new SystemException(
                      "Unable to retrieve storage connection string for application}", e);
            }
            finally
            {
                if ((channel != null) && (channel.State == CommunicationState.Opened)) channel.Close();
                if ((channelFactory != null) && (channelFactory.State == CommunicationState.Opened)) channelFactory.Close();
            }

            return config;
        }
    }
}
