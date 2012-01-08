using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.WindowsAzure.StorageClient;
using System.Diagnostics;

namespace CloudDAL
{
    [Serializable]
    public class QueueMessageFormatException : System.Exception
    {
        public String FieldName { get; private set; }
        public String FieldValue { get; private set; }
        public String FieldType { get; private set; }
        public QueueMessageFormatException() : base() { }
        public QueueMessageFormatException(string message) : base(message) { }
        public QueueMessageFormatException(string message, Exception inner) : base(message, inner) { }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected QueueMessageFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.FieldName = info.GetString("FieldName");
            this.FieldValue = info.GetString("FieldValue");
            this.FieldType = info.GetString("FieldType");
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("FieldName", FieldName);
            info.AddValue("FieldValue", FieldValue);
            info.AddValue("FieldType", FieldType);
            base.GetObjectData(info, context);
        }
        public QueueMessageFormatException(string fieldName, string fieldValue, Type fieldType)
            : base("Cannot convert message field to target type")
        {
            this.FieldName = fieldName;
            this.FieldValue = fieldValue;
            this.FieldType = fieldType.Name;
        }
    }

    public class QueueMessage
    {
        protected const char MSG_SEPARATOR = '\n';

        protected virtual Int32 MSG_COMPONENTS { get { return 0; } }

        private String _queueUri;
        private String _clientId;
        private String _requestId;
        protected List<String> _components;

        // common message components 
        public CloudQueueMessage RawMessage { get; private set; }
        public ImageProcessingQueue Queue { get; private set; }
        public String ClientId { get; private set; }
        public Guid RequestId { get; private set; }
        public String BlobName { get { return this._requestId.ToString(); } }

        // calculated property converting typed properties to string payload
        public String Payload
        {
            get
            {
                return String.Format("{0}{4}{1}{4}{2}{4}{3}",
                    this._queueUri,
                    this._clientId,
                    this._requestId,
                    String.Join(MSG_SEPARATOR.ToString(), this._components.ToArray()),
                    MSG_SEPARATOR);
            }
        }

        public QueueMessage() { }

        public void DeleteFromQueue()
        {
            // delete the message from the given queue
            if ((this.Queue.RawQueue != null) && (this.Queue.RawQueue.Exists()))
            {
                try
                {
                    this.Queue.RawQueue.DeleteMessage(this.RawMessage);
                }
                catch (StorageClientException sce)
                {
                    // if visibility timeout expired, there's chance message is being processed more than
                    // once, but message can only be deleted once.  Subsequent attempt to delete same
                    // message results in 404, but that's not really an error, so we'll ignore it.
                    if (sce.ErrorCode != StorageErrorCode.ResourceNotFound)
                    {
                        Trace.TraceError("Request: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                            default(Guid),
                            sce.GetType(),
                            String.Format("{0} [{1}]", sce.Message, this.RawMessage.Id),
                            sce.StackTrace,
                            Environment.NewLine);
                    }
                }
                catch (Exception e)
                {
                    Trace.TraceError("Request: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                        default(Guid),
                        e.GetType(),
                        String.Format("{0} [{1}]", e.Message, this.RawMessage.Id),
                        e.StackTrace,
                        Environment.NewLine);
                }
            }
        }

        // factory for message pulled off of queue
        internal static T CreateFromMessage<T>(CloudQueueMessage rawMsg) where T : QueueMessage, new()
        {
            // check if message parameter is valid
            if ((rawMsg == null) || String.IsNullOrEmpty(rawMsg.AsString))
                throw new ArgumentNullException("rawMsg", "No message data to parse");

            // create a new message instance
            T newQueueMessage = new T();
            newQueueMessage.RawMessage = rawMsg;

            // split message payload into array
            String[] s = newQueueMessage.RawMessage.AsString.Split(MSG_SEPARATOR);

            // first element is queue URI
            if (s.Length >= 3)
            {
                newQueueMessage._queueUri = s[0];
                newQueueMessage._clientId = s[1];
                newQueueMessage._requestId = s[2];

                // split payload array into components
                newQueueMessage._components = s.Skip(3).ToList();

                // parse into strongly typed message fields
                newQueueMessage.Parse();
            }
            else
            {
                throw new QueueMessageFormatException("Message is missing one or more required elements (queueUri, userId, requestId)");
            }

            // return the new message instance
            return newQueueMessage;
        }

        // factory for message to be dispatched onto queue
        internal static T CreateFromArguments<T>(Uri queueUri, String clientId, Guid requestId, params object[] parms) where T : QueueMessage, new()
        {
            T newQueueMessage = new T();

            // pull arguments into payload arrays
            newQueueMessage._queueUri = queueUri.ToString();
            newQueueMessage._clientId = clientId;
            newQueueMessage._requestId = requestId.ToString();
            newQueueMessage._components = (from p in parms select p.ToString()).ToList<String>();

            // parse into strongly typed message fields
            newQueueMessage.Parse();

            // return the new message instance
            return newQueueMessage;
        }

        // virtual function parses the common components, overrides in subclass parse the unique fields
        // both assume the _components string lists have been populated
        protected virtual void Parse()
        {
            // check component counts
            if (this._components.Count != MSG_COMPONENTS)
                throw new QueueMessageFormatException(
                    String.Format("Expected {0} message fields, found {1}", MSG_COMPONENTS, this._components.Count));

            // pass clientID through
            this.ClientId = this._clientId;

            // check if valid queue (queue Uri is found AND part of the account associated with given client)
            if (Uri.IsWellFormedUriString(this._queueUri, UriKind.Absolute))
            {
                this.Queue = QueueAccessor.FindQueue(this._queueUri);
                if (this.Queue == null)
                {
                    throw new QueueMessageFormatException(
                        String.Format("Queue {0} does not currently exist", this._queueUri, this.ClientId));
                }
            }
            else
            {
                throw new QueueMessageFormatException("Queue", this._queueUri, typeof(Uri));
            }

            // check if requestId is a GUID
            Guid g;
            if (Guid.TryParse(this._requestId, out g))
            {
                this.RequestId = g;
            }
            else
            {
                throw new QueueMessageFormatException("RequestId", this._requestId, typeof(Guid));
            }
        }
    }

    public class ImageRequestMessage : QueueMessage
    {
        public Uri ImageUri;
        public Uri ImageLibraryUri;
        public Byte TileSize;
        public Byte Slices;

        protected override Int32 MSG_COMPONENTS { get { return 4; } }

        public ImageRequestMessage() : base() { }

        protected override void Parse()
        {
            // parse the common message data first
            base.Parse();

            // parse the unique message data
            if (Uri.IsWellFormedUriString(this._components[0], UriKind.Absolute))
                this.ImageUri = new Uri(this._components[0]);
            else
                throw new QueueMessageFormatException("ImageUri", this._components[0], typeof(Uri));

            if (Uri.IsWellFormedUriString(this._components[1], UriKind.Absolute))
                this.ImageLibraryUri = new Uri(this._components[1]);
            else
                throw new QueueMessageFormatException("ImageLibraryUri", this._components[1], typeof(Uri));

            if (!Byte.TryParse(this._components[2], out this.TileSize))
                throw new QueueMessageFormatException("TileSize", this._components[2], typeof(Byte));

            if (!Byte.TryParse(this._components[3], out this.Slices))
                throw new QueueMessageFormatException("Slices", this._components[3], typeof(Byte));
        }
    }

    public class SliceRequestMessage : QueueMessage
    {
        public Uri ImageLibraryUri;
        public Uri SliceUri;
        public Byte SliceSequence;
        public Byte TileSize;

        protected override Int32 MSG_COMPONENTS { get { return 4; } }

        public SliceRequestMessage() : base() { }

        protected override void Parse()
        {
            // parse the common message data first
            base.Parse();

            // parse the unique message data
            if (Uri.IsWellFormedUriString(this._components[0], UriKind.Absolute))
                this.ImageLibraryUri = new Uri(this._components[0]);
            else
                throw new QueueMessageFormatException("ImageLibraryUri", this._components[0], typeof(Uri));


            if (Uri.IsWellFormedUriString(this._components[1], UriKind.Absolute))
                this.SliceUri = new Uri(this._components[1]);
            else
                throw new QueueMessageFormatException("SliceUri", this._components[1], typeof(Uri));

            if (!Byte.TryParse(this._components[2], out this.SliceSequence))
                throw new QueueMessageFormatException("SliceSequence", this._components[2], typeof(Byte));

            if (!Byte.TryParse(this._components[3], out this.TileSize))
                throw new QueueMessageFormatException("TileSize", this._components[3], typeof(Byte));
        }
    }

    public class SliceResponseMessage : QueueMessage
    {
        public Uri SliceUri;

        protected override Int32 MSG_COMPONENTS { get { return 1; } }

        public SliceResponseMessage() : base() { }

        protected override void Parse()
        {
            // parse the common message data first
            base.Parse();

            // parse the unique message data
            if (Uri.IsWellFormedUriString(this._components[0], UriKind.Absolute))
                this.SliceUri = new Uri(this._components[0]);
            else
                throw new QueueMessageFormatException("SliceUri", this._components[0], typeof(Uri));
        }
    }

    public class ImageResponseMessage : QueueMessage
    {
        public Uri ImageUri;

        protected override Int32 MSG_COMPONENTS { get { return 1; } }

        public ImageResponseMessage() : base() { }

        protected override void Parse()
        {
            base.Parse();

            // parse the unique message data
            if (Uri.IsWellFormedUriString(this._components[0], UriKind.Absolute))
                this.ImageUri = new Uri(this._components[0]);
            else
                throw new QueueMessageFormatException("ImageUri", this._components[0], typeof(Uri));
        }
    }
}
