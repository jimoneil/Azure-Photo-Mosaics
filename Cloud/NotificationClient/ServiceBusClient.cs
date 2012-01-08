using System;
using System.Diagnostics;
using System.ServiceModel;
using Microsoft.ServiceBus;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace NotificationServices
{
    [ServiceContract(Name = "NotificationContract")]
    public interface INotificationService
    {
        [OperationContract(IsOneWay = true)]
        void NotifyRequestCompleted(Guid requestId, Uri fullUri);

        [OperationContract(IsOneWay = true)]
        void NotifyRequestStarted(Guid requestId, String requestPath, Uri requestUri, Byte tileSize, DateTime submissionTime);
    }

    public interface INotificationChannel : INotificationService, IClientChannel { }

    public static class Notifier
    {
        private static ChannelFactory<INotificationChannel> GetChannelFactory(String clientId)
        {
            var sbConfig = InternalStorageBrokerServices.StorageBroker.GetServiceBusConfiguration(clientId);

            ServiceBusEnvironment.SystemConnectivity.Mode = ConnectivityMode.AutoDetect;

            Uri serviceUri = ServiceBusEnvironment.CreateServiceUri("https", sbConfig.Namespace, String.Format("NotificationService/{0}", clientId));

            TransportClientEndpointBehavior sbCredential = new TransportClientEndpointBehavior();
            sbCredential.TokenProvider = TokenProvider.CreateSharedSecretTokenProvider(sbConfig.Issuer, sbConfig.Secret);

            ChannelFactory<INotificationChannel> channelFactory =
                new ChannelFactory<INotificationChannel>(new BasicHttpRelayBinding(), new EndpointAddress(serviceUri));
            channelFactory.Endpoint.Behaviors.Add(sbCredential);

            return channelFactory;
        }

        public static void SendRequestCompletedNotification(String clientId, Guid requestId, Uri fullUri)
        {
            ChannelFactory<INotificationChannel> channelFactory = null;
            INotificationChannel channel = null;

            try
            {
                channelFactory = GetChannelFactory(clientId);
                channel = channelFactory.CreateChannel();

                channel.Open();

                channel.NotifyRequestCompleted(requestId, fullUri);
            }
            catch (Exception e)
            {
                Trace.TraceError("Client: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                    clientId,
                    e.GetType(),
                    e.Message,
                    e.StackTrace,
                    Environment.NewLine);
            }
            finally
            {
                if ((channel != null) && (channel.State == CommunicationState.Opened)) channel.Close();
                if ((channelFactory != null) && (channelFactory.State == CommunicationState.Opened)) channelFactory.Close();
            }
        }

        public static void SendRequestStartedNotification(String clientId, Guid requestId, String requestPath, Uri requestUri, Byte tileSize, DateTime submissionTime)
        {
            ChannelFactory<INotificationChannel> channelFactory = null;
            INotificationChannel channel = null;

            try
            {
                channelFactory = GetChannelFactory(clientId);
                channel = channelFactory.CreateChannel();

                channel.Open();

                channel.NotifyRequestStarted(requestId, requestPath, requestUri, tileSize, submissionTime);
            }
            catch (Exception e)
            {
                Trace.TraceError("Client: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                    clientId,
                    e.GetType(),
                    e.Message,
                    e.StackTrace,
                    Environment.NewLine);
            }
            finally
            {
                if ((channel != null) && (channel.State == CommunicationState.Opened)) channel.Close();
                if ((channelFactory != null) && (channelFactory.State == CommunicationState.Opened)) channelFactory.Close();
            }
        }
    }
}