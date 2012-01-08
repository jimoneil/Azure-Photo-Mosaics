Imports Microsoft.ServiceBus
Imports System.ServiceModel


<ServiceContract(Name:="NotificationContract")>
Interface INotificationService

    <OperationContract(IsOneWay:=True)>
    Sub NotifyRequestCompleted(requestId As Guid, fullUri As Uri)

    <OperationContract(IsOneWay:=True)>
    Sub NotifyRequestStarted(requestId As Guid, requestPath As String,
                             requestUri As Uri, tileSize As Byte, submissionTime As DateTime)
End Interface

Public Class NotificationService
    Implements INotificationService

    Public Sub NotifyRequestCompleted(ByVal requestId As Guid, fullUri As Uri) _
        Implements INotificationService.NotifyRequestCompleted

        For Each form In Application.OpenForms.OfType(Of AbstractNotificationForm)()
            form.Invoke(Sub(f As AbstractNotificationForm)
                            Dim e As New RequestCompletedEventArgs(requestId, fullUri)
                            f.OnRequestCompleted(Me, e)
                        End Sub, form)
        Next
    End Sub

    Public Sub NotifyRequestStarted(requestId As Guid, requestPath As String, requestUri As Uri, tileSize As Byte, startTime As DateTime) _
        Implements INotificationService.NotifyRequestStarted

        For Each form In Application.OpenForms.OfType(Of AbstractNotificationForm)()
            form.Invoke(Sub(f As AbstractNotificationForm)
                            Dim e As New RequestStartedEventArgs(requestId, requestPath, requestUri, tileSize, startTime)
                            f.OnRequestStarted(Me, e)
                        End Sub, form)
        Next
    End Sub
End Class


Public Class NotificationServiceEndpoint
    Private Shared Host As ServiceHost = Nothing

    Shared Sub Register(clientId As String)

        ' do nothing if host has already been registered
        If Host IsNot Nothing Then Return

        Try
            ' get service bus configuration
            Dim sbConfig = New AzureStorageBroker.StorageBrokerClient().GetServiceBusConfiguration(clientId)

            ' create service host
            Dim address = ServiceBusEnvironment.CreateServiceUri("https", sbConfig.Namespace,
                                        String.Format("NotificationService/{0}", clientId))
            Host = New ServiceHost(GetType(NotificationService), address)

            ' set credentials on each endpoint
            Dim sbCredential = New TransportClientEndpointBehavior()
            sbCredential.TokenProvider = TokenProvider.CreateSharedSecretTokenProvider(sbConfig.Issuer, sbConfig.Secret)

            For Each endpoint In Host.Description.Endpoints
                endpoint.Behaviors.Add(sbCredential)
            Next

            ' start the notification service
            Host.Open()

            ' start the n
        Catch aade As AddressAccessDeniedException
            Throw New System.Exception("Process does not have access to setup the ServiceBus namespace.  For testing, consider running under an Adminstrator account or run netsh from an elevated prompt to grant access to the current user account (cf. http://go.microsoft.com/fwlink/?LinkId=70353)")

        Catch ex As Exception
            Throw
        End Try
    End Sub

    Shared Sub Unregister()
        ' shut down the service w/5 second timeout
        If Host IsNot Nothing Then
            Host.Close(New TimeSpan(0, 0, 5))
            Host = Nothing
        End If
    End Sub
End Class