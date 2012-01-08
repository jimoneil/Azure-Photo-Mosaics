Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Net
Imports System.Threading
Imports Microsoft.WindowsAzure
Imports Microsoft.WindowsAzure.Diagnostics
Imports Microsoft.WindowsAzure.ServiceRuntime
Imports CloudDAL
Imports InternalStorageBrokerServices

Public Class WorkerRole
    Inherits RoleEntryPoint

    Public Overrides Sub Run()

        While (True)

            ' NEW SLICE REQUEST
            Dim requestMsg As SliceRequestMessage =
                If(QueueAccessor.IsAvailable, QueueAccessor.SliceRequestQueue.AcceptMessage(Of SliceRequestMessage)(), Nothing)
            If requestMsg IsNot Nothing Then

                ' check for possible poison message
                If requestMsg.RawMessage.DequeueCount <= requestMsg.Queue.PoisonThreshold Then

                    Try

                        ' instantiate storage accessors
                        Dim imgLibraryAccessor As BlobAccessor = New BlobAccessor(StorageBroker.GetStorageConnectionStringForAccount(requestMsg.ImageLibraryUri))
                        Dim blobAccessor As BlobAccessor = New BlobAccessor(StorageBroker.GetStorageConnectionStringForAccount(requestMsg.SliceUri))
                        Dim tableAccessor As TableAccessor = New TableAccessor(StorageBroker.GetStorageConnectionStringForClient(requestMsg.ClientId))

                        ' use caching? (this is a role property for experimentation, in production you'd likely commit to caching)
                        Dim cacheType As CachingMechanism
                        Try
                            cacheType = CType([Enum].Parse(cacheType.GetType(), RoleEnvironment.GetConfigurationSettingValue("CachingMechanism")), CachingMechanism)
                        Catch e As Exception
                            cacheType = CachingMechanism.InRole
                        End Try

                        ' time the operation
                        Dim sw As New Stopwatch()
                        sw.Start()

                        ' build the library
                        Dim library As New ImageLibrary(imgLibraryAccessor,
                                                        requestMsg.ImageLibraryUri,
                                                        requestMsg.TileSize,
                                                        cacheType)

                        sw.Stop()
                        tableAccessor.WriteStatusEntry(requestMsg.RequestId,
                                 String.Format("Slice {0}: CacheType: {1}, RoleId: {2}",
                                               requestMsg.SliceSequence, cacheType.ToString(), RoleEnvironment.CurrentRoleInstance.Id))
                        sw.Start()

                        ' generate the image
                        Dim generatedImage = library.Mosaicize(
                            blobAccessor.RetrieveImage(requestMsg.SliceUri),
                            requestMsg.TileSize,
                            Sub(msg As String)
                                tableAccessor.WriteStatusEntry(requestMsg.RequestId,
                                            String.Format("Slice {0}: {1}", requestMsg.SliceSequence, msg))
                            End Sub
                        )

                        ' stop the timer
                        sw.Stop()

                        tableAccessor.WriteStatusEntry(requestMsg.RequestId,
                                 String.Format("Slice {0} completed in {1}:{2:00}.{3:000} via RoleId: {4}",
                                               requestMsg.SliceSequence, sw.Elapsed.Minutes, sw.Elapsed.Seconds, sw.ElapsedMilliseconds,
                                               RoleEnvironment.CurrentRoleInstance.Id))
                        tableAccessor.WriteStatusEntry(requestMsg.RequestId,
                                 String.Format("Slice {0} generated Image hits: {1}/{2} ColorValue hits: {3}/{4}",
                                               requestMsg.SliceSequence, library.ImageRetrieves, library.ImageRequests,
                                               library.ColorValueRetrieves, library.ColorValueRequests))

                        ' store the processed segment
                        Dim processedSliceUri = blobAccessor.StoreImage(generatedImage, blobAccessor.SliceOutputContainer,
                                                                        String.Format("{0}_{1:000}", requestMsg.BlobName, requestMsg.SliceSequence))

                        ' signal completion of the slice
                        QueueAccessor.SliceResponseQueue.SubmitMessage(Of SliceResponseMessage)(requestMsg.ClientId, requestMsg.RequestId, processedSliceUri)

                        ' message has been processed
                        requestMsg.DeleteFromQueue()

                    Catch e As Exception

                        Dim tableAccessor As TableAccessor = New TableAccessor(StorageBroker.GetStorageConnectionStringForClient(requestMsg.ClientId))
                        tableAccessor.WriteStatusEntry(requestMsg.RequestId,
                            String.Format("Slice {0}: An unrecoverable exception has occurred, consult diagnostic logs for details.",
                                          requestMsg.SliceSequence))

                        ' something really bad happened!
                        Trace.TraceError("Request: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                            requestMsg.RequestId,
                            e.GetType(),
                            e.Message,
                            e.StackTrace,
                            Environment.NewLine)
                    End Try
                Else

                        ' remove poison message from the queue
                        requestMsg.DeleteFromQueue()
                        Trace.TraceWarning("Request: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                                requestMsg.RequestId,
                                "N/A",
                                String.Format("Possible poison message [id: {0}] removed from queue '{1}' after {2} attempts",
                                    requestMsg.RawMessage.Id,
                                    QueueAccessor.SliceRequestQueue,
                                    QueueAccessor.SliceRequestQueue.PoisonThreshold),
                                "N/A",
                                Environment.NewLine)
                End If
            Else
                Thread.Sleep(10000)
            End If
        End While
    End Sub

    Public Overrides Function OnStart() As Boolean

        ' Set the maximum number of concurrent connections 
        ServicePointManager.DefaultConnectionLimit = 12

        ' For information on handling configuration changes
        ' see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

        Return MyBase.OnStart()
    End Function
End Class
