using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading;
using CloudDAL;
using InternalStorageBrokerServices;
using Microsoft.WindowsAzure.ServiceRuntime;
using NotificationServices;
using Utilities;

namespace AzureJobController
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            TableAccessor tableAccessor = null;
            BlobAccessor blobAccessor = null;

            while (true)
            {
                #region NEW IMAGE REQUEST
                var requestMsg = QueueAccessor.IsAvailable ? QueueAccessor.ImageRequestQueue.AcceptMessage<ImageRequestMessage>() : null;
                if (requestMsg != null)
                {
                    // check for possible poison message
                    if (requestMsg.RawMessage.DequeueCount <= requestMsg.Queue.PoisonThreshold)
                    {
                        try
                        {
                            // instantiate storage accessors
                            blobAccessor = new BlobAccessor(StorageBroker.GetStorageConnectionStringForAccount(requestMsg.ImageUri));
                            tableAccessor = new TableAccessor(StorageBroker.GetStorageConnectionStringForClient(requestMsg.ClientId));

                            // retrieve source image from blob storage
                            var imageBytes = blobAccessor.RetrieveImage(requestMsg.ImageUri);

                            // actual slices may be less than request if image is too small
                            var slices = ImageUtilities.SliceAndDice(imageBytes, requestMsg.Slices).ToList();
                            Byte totalSlices = (Byte)slices.Count;
                            if (requestMsg.Slices != totalSlices)
                            {
                                var job = tableAccessor.GetJobByRequestId(requestMsg.RequestId);
                                if (job != null)
                                {
                                    job.Slices = totalSlices;
                                    tableAccessor.UpdateJob(job);
                                }
                            }

                            // loop through each slice, storing it and submitting a message to queue
                            for (Byte sliceNum = 0; sliceNum < totalSlices; sliceNum++)
                            {
                                Uri slicePath = blobAccessor.StoreImage(
                                    slices[sliceNum], BlobAccessor.SliceInputContainer, String.Format("{0}_{1:000}", requestMsg.BlobName, sliceNum));

                                QueueAccessor.SliceRequestQueue.SubmitMessage<SliceRequestMessage>
                                    (requestMsg.ClientId, requestMsg.RequestId, requestMsg.ImageLibraryUri, slicePath, sliceNum, requestMsg.TileSize);

                                tableAccessor.WriteStatusEntry(requestMsg.RequestId,
                                    String.Format("Dispatched slice {0} of {1}", sliceNum, totalSlices - 1));
                            }

                            // message processed successfully
                            requestMsg.DeleteFromQueue();
                        }
                        catch (Exception e)
                        {
                            // something really bad happened!
                            Trace.TraceError("Request: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                                requestMsg.RequestId,
                                e.GetType(),
                                e.Message,
                                e.StackTrace,
                                Environment.NewLine);
                        }
                    }
                    else
                    {
                        // remove poison message from the queue
                        requestMsg.DeleteFromQueue();
                        Trace.TraceWarning("Request: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                                requestMsg.RequestId,
                                "N/A",
                                String.Format("Possible poison message [id: {0}] removed from queue '{1}' after {2} attempts", 
                                    requestMsg.RawMessage.Id,
                                    QueueAccessor.ImageRequestQueue,
                                    QueueAccessor.ImageRequestQueue.PoisonThreshold),
                                "N/A",
                                Environment.NewLine);
                    }
                }
                #endregion

                #region COMPLETED SLICE NOTIFICATION
                var responseMsg = QueueAccessor.IsAvailable ? QueueAccessor.SliceResponseQueue.AcceptMessage<SliceResponseMessage>() : null;
                if (responseMsg != null)
                {
                    // check for possible poison message
                    if (responseMsg.RawMessage.DequeueCount <= responseMsg.Queue.PoisonThreshold)
                    {
                        try
                        {
                            // instantiate storage accessors
                            blobAccessor = new BlobAccessor(StorageBroker.GetStorageConnectionStringForAccount(responseMsg.SliceUri));
                            tableAccessor = new TableAccessor(StorageBroker.GetStorageConnectionStringForClient(responseMsg.ClientId));

                            // get the job data (from Azure table) indicating number of slices and the tile size
                            var job = tableAccessor.GetJobByRequestId(responseMsg.RequestId);
                            if ((job != null) && (job.Slices > 0) && (job.TileSize > 0) && (job.SourceWidth > 0) && (job.SourceHeight > 0))
                            {
                                // get slices processed so far
                                var processedSliceUris = blobAccessor.RetrieveImageUris(BlobAccessor.SliceOutputContainer, responseMsg.BlobName).ToList();

                                // if we have the expected number, this is the last slice to complete
                                if (processedSliceUris.Count() == (Int32)job.Slices)
                                {
                                    Byte[] finalImgBytes = ImageUtilities.StitchAndMend(
                                        processedSliceUris, blobAccessor.RetrieveImages,
                                        (byte)job.TileSize, new Size(job.SourceWidth, job.SourceHeight));

                                    Uri finalImageUri = blobAccessor.StoreImage(finalImgBytes, BlobAccessor.ImageOutputContainer, responseMsg.BlobName);

                                    QueueAccessor.ImageResponseQueue.SubmitMessage<ImageResponseMessage>
                                        (responseMsg.ClientId, responseMsg.RequestId, finalImageUri);
                                }
                            }
                            else
                            {
                                throw new SystemException("Metadata for job {0} in Jobs table was not found or is invalid");
                            }

                            // message processed successfully
                            responseMsg.DeleteFromQueue();
                        }
                        catch (Exception e)
                        {

                            // something really bad happened!
                            Trace.TraceError("Request: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                                responseMsg.RequestId,
                                e.GetType(),
                                e.Message,
                                e.StackTrace,
                                Environment.NewLine);
                        }
                    }
                    else
                    {
                        // remove poison message from the queue
                        responseMsg.DeleteFromQueue();
                        Trace.TraceWarning("Request: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                                responseMsg.RequestId,
                                "N/A",
                                String.Format("Possible poison message [id: {0}] removed from queue '{1}' after {2} attempts", 
                                    responseMsg.RawMessage.Id,
                                    QueueAccessor.SliceResponseQueue,
                                    QueueAccessor.SliceResponseQueue.PoisonThreshold),
                                "N/A",
                                Environment.NewLine);
                    }
                }
                #endregion

                #region MOST SINCERELY DONE NOTIFICATION
                var completeMsg = QueueAccessor.IsAvailable ? QueueAccessor.ImageResponseQueue.AcceptMessage<ImageResponseMessage>() : null;
                if (completeMsg != null) 
                {
                    // check for possible poison message
                    if (completeMsg.RawMessage.DequeueCount <= completeMsg.Queue.PoisonThreshold)
                    {
                        try
                        {
                            // instantiate storage accessors
                            blobAccessor = new BlobAccessor(StorageBroker.GetStorageConnectionStringForAccount(completeMsg.ImageUri));
                            tableAccessor = new TableAccessor(StorageBroker.GetStorageConnectionStringForClient(completeMsg.ClientId));

                            // get the completed job entity from table storage
                            var job = tableAccessor.GetJobByRequestId(completeMsg.RequestId);
                            if (job != null)
                            {

                                // update pointer to completed image (and the finish time)
                                job.FinalImageUri = completeMsg.ImageUri.ToString();
                                job.FinishTime = DateTime.UtcNow;

                                tableAccessor.UpdateJob(job);

                                tableAccessor.WriteStatusEntry(completeMsg.RequestId, "Request complete.");
                            }
                            else
                            {
                                Trace.TraceWarning("Request: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                                    completeMsg.RequestId,
                                    "N/A",
                                    "Request completed, but record not found in Jobs table",
                                    "N/A",
                                    Environment.NewLine);
                            }

                            // only notify user (or try to) once!
                            if (completeMsg.RawMessage.DequeueCount == 1)
                            {
                                try
                                {
                                    // notify via service bus that request is complete
                                    if (job != null)
                                    {
                                        Notifier.SendRequestCompletedNotification(
                                            job.ClientId,
                                            completeMsg.RequestId,
                                            completeMsg.ImageUri);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Trace.TraceWarning("Request: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                                        completeMsg.RequestId,
                                        e.GetType(),
                                        e.Message,
                                        e.StackTrace,
                                        Environment.NewLine);
                                }
                            }

                            // message processed successfully
                            completeMsg.DeleteFromQueue();
                        }

                        catch (Exception e)
                        {
                            // something really bad happened!
                            Trace.TraceError("Request: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                                completeMsg.RequestId,
                                e.GetType(),
                                e.Message,
                                e.StackTrace,
                                Environment.NewLine);
                        }
                    }
                    else
                    {
                        // remove poison m essage from the queue
                        completeMsg.DeleteFromQueue();
                        Trace.TraceWarning("Request: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                                completeMsg.RequestId,
                                "N/A",
                                String.Format("Possible poison message [id: {0}] removed from queue '{1}' after {2} attempts", 
                                    completeMsg.RawMessage.Id,
                                    QueueAccessor.ImageResponseQueue,
                                    QueueAccessor.ImageResponseQueue.PoisonThreshold),
                                "N/A",
                                Environment.NewLine);
                    }
                }
                #endregion

                // only sleep if we didn't do anything this time through
                if ((requestMsg == null) && (responseMsg == null) && (completeMsg == null))
                {
                    Thread.Sleep(10000);
                }
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}