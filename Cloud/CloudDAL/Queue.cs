using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

namespace CloudDAL
{
    public class ImageProcessingQueue
    {
        public readonly CloudQueue RawQueue = null;
        public readonly String Name;
        public readonly TimeSpan Timeout;
        public readonly Int32 PoisonThreshold;

        internal ImageProcessingQueue(CloudQueueClient queueClient, String queueName)
        {
            this.Name = queueName;
            this.RawQueue = queueClient.GetQueueReference(queueName);

            // fetch the queue's attributes (metadata)
            this.RawQueue.FetchAttributes();
            String timeout = this.RawQueue.Metadata["defaulttimeout"];
            String threshold = this.RawQueue.Metadata["poisonthreshold"];

            // pull out queue-specific timeout/poison message retry value (or set to defaults)
            Int32 i;
            this.Timeout = (Int32.TryParse(timeout, out i)) ? new TimeSpan(0, 0, i) : new TimeSpan(0, 0, 30);
            this.PoisonThreshold = (Int32.TryParse(threshold, out i)) ? i : Int32.MaxValue;
        }

        public void SubmitMessage<T>(String clientId, Guid requestId, params object[] parms) where T : QueueMessage, new()
        {
            T parsedMsg = default(T);

            this.RawQueue.CreateIfNotExist();
            try
            {
                parsedMsg = QueueMessage.CreateFromArguments<T>(this.RawQueue.Uri, clientId, requestId, parms);
                this.RawQueue.AddMessage(new CloudQueueMessage(parsedMsg.Payload));
            }
            catch (QueueMessageFormatException qfme)
            {
                if (String.IsNullOrEmpty(qfme.FieldName))
                {
                    Trace.TraceError("Request: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                        default(Guid),
                        qfme.GetType(),
                        qfme.Message,
                        qfme.StackTrace,
                        Environment.NewLine);
                }
                else
                {
                    Trace.TraceError("Request: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                        default(Guid),
                        qfme.GetType(),
                        String.Format("{0} [FieldName: {1}, FieldValue: {2}, FieldType: {3}]",
                                       qfme.Message, qfme.FieldName, qfme.FieldValue, qfme.FieldType),
                        qfme.StackTrace,
                        Environment.NewLine);
                }
            }
        }

        public T AcceptMessage<T>() where T : QueueMessage, new()
        {
            T parsedMsg = default(T);
            if (this.RawQueue.Exists())
            {
                CloudQueueMessage rawMsg = this.RawQueue.GetMessage(this.Timeout);
                if (rawMsg != null)
                {
                    try
                    {
                        parsedMsg = QueueMessage.CreateFromMessage<T>(rawMsg);
                    }
                    catch (QueueMessageFormatException qfme)
                    {
                        if (String.IsNullOrEmpty(qfme.FieldName))
                        {
                            Trace.TraceError("Request: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                                default(Guid),
                                qfme.GetType(),
                                qfme.Message,
                                qfme.StackTrace,
                                Environment.NewLine);
                        }
                        else
                        {
                            Trace.TraceError("Request: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                                default(Guid),
                                qfme.GetType(),
                                String.Format("{0} [FieldName: {1}, FieldValue: {2}, FieldType: {3}]",
                                               qfme.Message, qfme.FieldName, qfme.FieldValue, qfme.FieldType),
                                qfme.StackTrace,
                                Environment.NewLine);
                        }
                    }
                }
            }
            return parsedMsg;
        }

        public override string ToString()
        {
            return (this.RawQueue == null) ? String.Empty : this.RawQueue.Uri.ToString();
        }
    }

    public static class QueueAccessor
    {
        public static readonly Boolean IsAvailable = false;
        public static readonly ImageProcessingQueue ImageRequestQueue = null;
        public static readonly ImageProcessingQueue SliceRequestQueue = null;
        public static readonly ImageProcessingQueue SliceResponseQueue = null;
        public static readonly ImageProcessingQueue ImageResponseQueue = null;
        private static ImageProcessingQueue[] _queueList = { };

        private static CloudQueueClient _queueClient = null;

        static QueueAccessor()
        {
            try
            {
                String connString = RoleEnvironment.GetConfigurationSettingValue("ApplicationStorageConnectionString");
                _queueClient = CloudStorageAccount.Parse(connString).CreateCloudQueueClient();

                ImageRequestQueue = new ImageProcessingQueue(_queueClient, "imagerequest");
                SliceRequestQueue = new ImageProcessingQueue(_queueClient, "slicerequest");
                SliceResponseQueue = new ImageProcessingQueue(_queueClient, "sliceresponse");
                ImageResponseQueue = new ImageProcessingQueue(_queueClient, "imageresponse");

                _queueList = new ImageProcessingQueue[] { ImageRequestQueue, SliceRequestQueue, SliceResponseQueue, ImageResponseQueue };

                IsAvailable = true;
            }
            catch (Exception e)
            {
                IsAvailable = false;
                Trace.TraceError("Request: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                    default(Guid),
                    e.GetType(),
                    e.Message,
                    e.StackTrace,
                    Environment.NewLine);
            }
        }

        public static ImageProcessingQueue FindQueue(String uri)
        {
            return (from q in _queueList
                    where q.RawQueue.Uri.ToString().Equals(uri, StringComparison.Ordinal)
                    select q).FirstOrDefault();
        }
    }
}