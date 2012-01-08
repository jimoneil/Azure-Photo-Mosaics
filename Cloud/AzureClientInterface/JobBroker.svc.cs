using System;
using System.Diagnostics;
using CloudDAL;
using NotificationServices;

namespace AzureClientInterface
{
    public class JobBroker : IJobBroker
    {
        public void SubmitImageForProcessing(String clientId, Guid requestId, Uri imageLibraryUri, String originalLocation, Byte tileSize, Byte slices, byte[] imgBytes)
        {
            Uri imageUri = null;
            BlobAccessor blobAccessor = null;
            TableAccessor tableAccessor = null;

            try
            {
                String connString = new StorageBrokerInternal().GetStorageConnectionStringForClient(clientId);
                tableAccessor = new TableAccessor(connString);
                blobAccessor = new BlobAccessor(connString);

                imageUri = blobAccessor.StoreImage(imgBytes, BlobAccessor.ImageInputContainer, requestId.ToString());
                var job = tableAccessor.CreateJob(clientId,
                                                    requestId,
                                                    originalLocation,
                                                    imageUri,
                                                    Utilities.ImageUtilities.GetImageSize(imgBytes),
                                                    tileSize,
                                                    slices);

                if (QueueAccessor.IsAvailable)
                {
                    QueueAccessor.ImageRequestQueue.SubmitMessage<ImageRequestMessage>
                            (clientId, requestId, imageUri, imageLibraryUri, tileSize, slices);

                    tableAccessor.WriteStatusEntry(requestId,
                        String.Format("New request from {0}: {1}", clientId, originalLocation));

                    try
                    {
                        Notifier.SendRequestStartedNotification(
                            clientId,
                            requestId,
                            originalLocation,
                            imageUri,
                            tileSize,
                            job.StartTime);
                    }
                    catch (Exception e)
                    {
                        Trace.TraceWarning("Request: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                            requestId,
                            e.GetType(),
                            e.Message,
                            e.StackTrace,
                            Environment.NewLine);
                    }
                }
                else
                {
                    throw new SystemException("Application queueing service could not be initialized; request cannot be processed");
                }
            }
            catch (Exception e)
            {
                // clean up if there was a problem
                if (blobAccessor != null) blobAccessor.DeleteImage(imageUri);

                Trace.TraceError("Request: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                                            requestId,
                                            e.GetType(),
                                            e.Message,
                                            e.StackTrace,
                                            Environment.NewLine);

                // surface exception to client
                throw;
            }
        }
    }
}
