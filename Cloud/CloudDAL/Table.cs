using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using System.Runtime.Serialization;

namespace CloudDAL
{
    public class StatusEntry : TableServiceEntity
    {
        public Guid RequestId { get; set; }
        public String RoleId { get; set; }
        public String Message { get; set; }

        public StatusEntry() { }

        public StatusEntry(Guid requestId, String roleId, String msg)
        {
            this.RequestId = requestId;
            this.RoleId = roleId;
            this.Message = msg;

            this.PartitionKey = requestId.ToString();
            this.RowKey = String.Format("{0}|{1}", DateTime.UtcNow.Ticks, Guid.NewGuid());
        }
    }

    public class JobEntry : TableServiceEntity
    {
        public Guid RequestId { get; set; }
        public String ClientId { get; set; }
        public String SourceImage { get; set; }
        public String SourceImageUri { get; set; }
        public Int32 SourceWidth { get; set; }
        public Int32 SourceHeight { get; set; }
        public Int32 TileSize { get; set; }
        public Int32 Slices { get; set; }
        public String FinalImageUri { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }

        public JobEntry() { }

        public JobEntry(String clientId, Guid requestId, String sourceImage, Uri sourceImageUri, Int32 width, Int32 height, Byte tileSize, Byte slices)
        {
            this.ClientId = clientId;
            this.RequestId = requestId;
            this.SourceImage = sourceImage;
            this.SourceImageUri = sourceImageUri.ToString();
            this.SourceHeight = height;
            this.SourceWidth = width;
            this.TileSize = tileSize;
            this.Slices = slices;
            this.StartTime = System.DateTime.UtcNow;
            this.FinishTime = DateTime.FromFileTimeUtc(0);
            this.FinalImageUri = String.Empty;

            this.PartitionKey = clientId;
            this.RowKey = String.Format("{0:D20}_{1}", this.StartTime.Ticks, requestId);
        }
    }

    public class TableAccessor
    {
        private CloudTableClient _tableClient = null;

        private TableContext _context = null;
        public TableContext Context
        {
            get
            {
                if (_context == null)
                    _context = new TableContext(_tableClient.BaseUri.ToString(), _tableClient.Credentials);
                return _context;
            }
        }

        public TableAccessor(String connectionString)
        {
            _tableClient = CloudStorageAccount.Parse(connectionString).CreateCloudTableClient();
        }
        
        public void WriteStatusEntry(Guid requestId, String message)
        {

            _tableClient.CreateTableIfNotExist("status");
            if (_tableClient.DoesTableExist("status"))
            {
                Context.AddObject("status", new StatusEntry(requestId, RoleEnvironment.CurrentRoleInstance.Id, message));
                Context.SaveChanges();
            }
        }

        public IEnumerable<StatusEntry> GetStatusEntriesForJob(Guid jobId)
        {
            if (_tableClient.DoesTableExist("status"))
            {
                CloudTableQuery<StatusEntry> qry =
                    (from s in Context.StatusEntries
                     where s.RequestId == jobId
                     select s).AsTableServiceQuery<StatusEntry>();
                return qry.Execute();
            }
            else
                return null;
        }

        public IEnumerable<JobEntry> GetJobsForClient(String clientId, ResultContinuation token = null)
        {
            if (_tableClient.DoesTableExist("jobs"))
            {
                CloudTableQuery<JobEntry> qry =
                       (from j in Context.Jobs
                        where j.PartitionKey == clientId
                        select j).AsTableServiceQuery<JobEntry>();

                // this will return ALL jobs for given client - alternative is to use BeginExecuteSegmented and
                // manage pagination via continuation tokens or filter criteria on client side
                return qry.Execute();
            }
            else
                return null;
        }

        public JobEntry GetJobByRequestId(Guid requestId)
        {

            if (_tableClient.DoesTableExist("jobs"))
            {
                return (from j in Context.Jobs
                       where j.RequestId == requestId
                       select j).FirstOrDefault();
            }
            else
                return null;
        }

        public JobEntry CreateJob(String clientId, Guid requestId, String srcImage, Uri srcImageUri, System.Drawing.Size srcSize, Byte tileSize, Byte slices)
        {
            _tableClient.CreateTableIfNotExist("jobs");
            if (_tableClient.DoesTableExist("jobs"))
            {
                var job = new JobEntry(clientId, requestId, srcImage, srcImageUri, srcSize.Width, srcSize.Height, tileSize, slices);
                Context.AddObject("jobs", job);
                Context.SaveChanges();

                return job;
            }
            else
                return null;
        }

        public void UpdateJob(JobEntry job)
        {
            // assumes job object was obtained from the context
            if (_tableClient.DoesTableExist("jobs"))
            {
                Context.UpdateObject(job);
                Context.SaveChanges();
            }
        }
    }

    public class TableContext : TableServiceContext
    {

        public TableContext(String baseAddress, StorageCredentials credentials)
            : base(baseAddress, credentials)
        {
        }

        public IQueryable<StatusEntry> StatusEntries
        {
            get
            {
                return this.CreateQuery<StatusEntry>("status");
            }
        }

        public IQueryable<JobEntry> Jobs
        {
            get
            {
                return this.CreateQuery<JobEntry>("jobs");
            }
        }
    }
}
