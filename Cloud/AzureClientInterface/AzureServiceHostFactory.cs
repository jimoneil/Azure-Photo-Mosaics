using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Web;

namespace AzureClientInterface
{
    public class AzureServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            try
            {
                String factoryClass = this.GetType().Name;
                String endpointName =
                    String.Format("{0}HttpEndpoint",
                            factoryClass.Substring(0, factoryClass.IndexOf("ServiceHostFactory")));
                String externalEndpointAddress =
                    string.Format("http://{0}/{1}.svc",
                    RoleEnvironment.CurrentRoleInstance.InstanceEndpoints[endpointName].IPEndpoint,
                    serviceType.Name);

                var host = new ServiceHost(serviceType, new Uri(externalEndpointAddress));

                var basicBinding = new BasicHttpBinding(String.Format("{0}Binding", serviceType.Name));
                foreach (var intf in serviceType.GetInterfaces())
                    host.AddServiceEndpoint(intf, basicBinding, String.Empty);

                return host;
            }
            catch (Exception e)
            {
                Trace.TraceError("ServiceFactory: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                               this.GetType().Name,
                               e.GetType(),
                               e.Message,
                               e.StackTrace,
                               Environment.NewLine);
                throw new SystemException(String.Format("Unable to activate service: {0}", serviceType.FullName), e);
            }
        }
    }

    public class ExternalServiceHostFactory : AzureServiceHostFactory { }
    public class InternalServiceHostFactory : AzureServiceHostFactory { }
}