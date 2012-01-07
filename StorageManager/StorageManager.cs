using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using FlickrNet;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace StorageManager
{
    public partial class StorageManager : Form
    {
        // objects required for PhotoMosaic application
        string[] tableList = { "status", "jobs" };
        string[] queueList = { "imagerequest", "imageresponse", "slicerequest", "sliceresponse" };
        string[] containerList = { "imageinput", "imageoutput", "sliceinput", "sliceoutput" };

        // queue metadata
        const String QUEUE_POISONTHRESHOLD = "3";
        const String QUEUE_INVISIBILITYTIMEOUT = "900";   // 15 minutes (in seconds)

        // image lists
        List<TilePhotoLibrary> imagesList =
            new List<TilePhotoLibrary>() { 
                new TilePhotoLibrary("Flickr", GetFlickrImage),
                new TilePhotoLibrary("MyPictures", GetMyPicturesImage) };
        const Int32 IMAGES_MAXPERLIBRARY = 300;     // max number of images to be transferred from image source
        const Int32 IMAGES_MAXSIZE = 1024 * 250;    // max image size (bytes) to be uploaded from your MyPictures directory

        // checkbox list in UI
        List<CheckBox> imagesListCheckBoxes = null;
        List<CheckBox> requiredStorageCheckBoxes = null;

        // StorageClient references
        CloudBlobClient cbc = null;
        CloudTableClient ctc = null;
        CloudQueueClient cqc = null;

        public StorageManager()
        {
            InitializeComponent();

            txtAccount.Text = Properties.Settings.Default.AccountName;
            txtKey.Text = Properties.Settings.Default.AccountKey;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            var credentials = new StorageCredentialsAccountAndKey(txtAccount.Text, txtKey.Text);

            ctc = new CloudTableClient(String.Format("https://{0}.table.core.windows.net/", txtAccount.Text), credentials);
            cqc = new CloudQueueClient(String.Format("https://{0}.queue.core.windows.net/", txtAccount.Text), credentials);
            cbc = new CloudBlobClient(String.Format("https://{0}.blob.core.windows.net/", txtAccount.Text), credentials);

            lbStatus.Items.Clear();

            this.UseWaitCursor = true;

            Action action = null;
            if (rbCreate.Checked) action = Create;
            if (rbDelete.Checked) action = Delete;
            if (rbCreateLibrary.Checked) action = CreateLibraries;

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, e2) => action();
            bw.RunWorkerCompleted += (s, e3) =>
                {
                    this.UseWaitCursor = false;
                    if (e3.Error != null)
                        MessageBox.Show(e3.Error.Message, "Storage Manager", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                };
            bw.RunWorkerAsync();
        }

        private void Create()
        {
            try
            {
                // create the blob containers
                if (chkRequiredContainers.Checked)
                {
                    foreach (var c in containerList)
                    {
                        var container = new CloudBlobContainer(c, cbc);
                        try
                        {
                            container.Create();
                            container.SetPermissions(
                                new BlobContainerPermissions() { PublicAccess = BlobContainerPublicAccessType.Blob });
                            this.Invoke((Action)(() => lbStatus.Items.Add(String.Format("Created container '{0}'", c))));
                        }
                        catch (StorageClientException sce)
                        {
                            this.Invoke((Action)(() => lbStatus.Items.Add(String.Format("{0} [container: {1}]", sce.Message, c))));
                        }
                    }
                }

                // create the tables
                if (chkRequiredTables.Checked)
                {
                    foreach (var t in tableList)
                    {
                        try
                        {
                            ctc.CreateTable(t);
                            this.Invoke((Action)(() => lbStatus.Items.Add(String.Format("Created table '{0}'", t))));
                        }
                        catch (StorageClientException sce)
                        {
                            this.Invoke((Action)(() => lbStatus.Items.Add(String.Format("{0} [table: {1}]", sce.Message, t))));
                        }
                    }
                }

                // create the queues
                if (chkRequiredQueues.Checked)
                {
                    foreach (var q in queueList)
                    {
                        var queue = cqc.GetQueueReference(q);
                        try
                        {
                            queue.Create();
                            queue.Metadata["PoisonThreshold"] = QUEUE_POISONTHRESHOLD;
                            queue.Metadata["DefaultTimeout"] = QUEUE_INVISIBILITYTIMEOUT;
                            queue.SetMetadata();
                            this.Invoke((Action)(() => lbStatus.Items.Add(String.Format("Created queue '{0}'", q))));
                        }
                        catch (StorageClientException sce)
                        {
                            this.Invoke((Action)(() => lbStatus.Items.Add(String.Format("{0} [queue: {1}]", sce.Message, q))));
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this.Invoke((Action)(() => lbStatus.Items.Add("**** CREATE COMPLETED ****")));
            }
        }

        private void Delete()
        {
            try
            {
                // delete containers (and blobs within them)
                foreach (var c in containerList)
                {
                    try
                    {
                        cbc.GetContainerReference(c).Delete();
                        this.Invoke((Action)(() => lbStatus.Items.Add(String.Format("Deleted container '{0}'", c))));
                    }
                    catch (StorageClientException sce)
                    {
                        this.Invoke((Action)(() => lbStatus.Items.Add(String.Format("{0} [container: {1}]", sce.Message, c))));
                    }
                }

                // delete tables
                foreach (var t in tableList)
                {
                    try
                    {
                        ctc.DeleteTable(t);
                        this.Invoke((Action)(() => lbStatus.Items.Add(String.Format("Deleted table '{0}'", t))));
                    }
                    catch (StorageClientException sce)
                    {
                        this.Invoke((Action)(() => lbStatus.Items.Add(String.Format("{0}: {1} [table: {2}]", sce.InnerException.Message, sce.StatusCode.ToString(), t))));
                    }
                }

                // delete image libraries
                foreach (var i in imagesList)
                {
                    try
                    {
                        cbc.GetContainerReference(i.ContainerName.ToLower()).Delete();
                        this.Invoke((Action)(() => lbStatus.Items.Add(String.Format("Deleted container '{0}'", i.ContainerName))));
                    }
                    catch (StorageClientException sce)
                    {
                        this.Invoke((Action)(() => lbStatus.Items.Add(String.Format("{0} [container: {1}]", sce.Message, i.ContainerName))));
                    }
                }

                // delete queues
                foreach (var q in queueList)
                {
                    try
                    {
                        cqc.GetQueueReference(q).Delete();
                        this.Invoke((Action)(() => lbStatus.Items.Add(String.Format("Deleted queue: '{0}'", q))));
                    }
                    catch (StorageClientException sce)
                    {
                        this.Invoke((Action)(() => lbStatus.Items.Add(String.Format("{0} [queue: {1}]", sce.Message, q))));
                    }
                }

                this.Invoke((Action)(() => lbStatus.Items.Add("Deletions aren't immediate.  Wait a few minutes if you intend to recreate the storage objects")));
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this.Invoke((Action)(() => lbStatus.Items.Add("**** DELETE COMPLETED ****")));
            }
        }

        // create image libraries
        private void CreateLibraries()
        {
            // fore each image library
            foreach (var i in imagesList)
            {
                if (i.BoundCheckBox == null || !i.BoundCheckBox.Checked) continue;

                // create a blob container
                var container = new CloudBlobContainer(i.ContainerName.ToLower(), cbc);
                try
                {
                    // set metadata
                    container.CreateIfNotExist();
                    container.SetPermissions(
                        new BlobContainerPermissions() { PublicAccess = BlobContainerPublicAccessType.Blob });
                    container.Metadata["ImageLibraryDescription"] = "Image tile library for PhotoMosaics";
                    container.SetMetadata();
                    this.Invoke((Action)(() => lbStatus.Items.Add(String.Format("Container '{0}' created (or already existed)", i.ContainerName))));

                    // loop through photos in collection
                    foreach (var p in i.PhotoEnumerator().Take(IMAGES_MAXPERLIBRARY))
                    {

                        // convert to URI (could be local path or URI)
                        var uri = new Uri(p.Path);

                        try
                        {
                            // get a memory stream
                            using (var memStream = new MemoryStream())
                            {

                                // get file contents as memory stream
                                if (uri.IsFile)
                                {
                                    var fs = new FileStream(p.Path, FileMode.Open, FileAccess.Read);
                                    fs.CopyTo(memStream);
                                }
                                else
                                {
                                    var request = WebRequest.Create(p.Path);
                                    var response = request.GetResponse();
                                    response.GetResponseStream().CopyTo(memStream);
                                }

                                // upload to blob
                                var blobRef = new CloudBlob(String.Format("{0}/{1}", i.ContainerName.ToLower(), p.Id), cbc);
                                blobRef.Properties.ContentType = "image/jpeg";
                                blobRef.Metadata["ImageSource"] = p.Path;
                                blobRef.UploadByteArray(memStream.GetBuffer());
                            }
                            this.Invoke((Action)(() => lbStatus.Items.Add(String.Format("  Added photo '{0}'", p.Path))));
                        }
                        catch (Exception e)
                        {
                            this.Invoke((Action)(() => lbStatus.Items.Add(e.Message)));
                        }
                    }
                }
                catch (StorageClientException sce)
                {
                    this.Invoke((Action)(() => lbStatus.Items.Add(String.Format("{0} [container: {1}]", sce.Message, i.ContainerName))));
                }
                catch (Exception e)
                {
                    this.Invoke((Action)(() => lbStatus.Items.Add(String.Concat("  ", e.Message))));
                }
            }

            this.Invoke((Action)(() => lbStatus.Items.Add("**** CREATE LIBRARY COMPLETED ****")));
        }

        // Image Library helper classes
        private class TilePhotoLibrary
        {
            public string ContainerName { get; private set; }
            public Func<IEnumerable<TilePhotoInfo>> PhotoEnumerator { get; private set; }
            public CheckBox BoundCheckBox { get; set; }
            public TilePhotoLibrary(String c, Func<IEnumerable<TilePhotoInfo>> e)
            {
                ContainerName = c;
                PhotoEnumerator = e;
                BoundCheckBox = null;
            }
        }
        private class TilePhotoInfo
        {
            public String Id { get; private set; }
            public String Path { get; private set; }
            public TilePhotoInfo(String i, String p)
            {
                Id = i;
                Path = p;
            }
        }

        // enumerator for populating Image Libary with Flickr
        private static IEnumerable<TilePhotoInfo> GetFlickrImage()
        {
            Flickr flickr = new Flickr(Properties.Settings.Default.FlickrAPIKey);

            PhotoCollection photos = flickr.InterestingnessGetList(PhotoSearchExtras.All, 1, 500);
            foreach (var p in photos)
            {
                if (!String.IsNullOrEmpty(p.SquareThumbnailUrl))
                    yield return new TilePhotoInfo(p.PhotoId, p.SquareThumbnailUrl);
            }
        }

        // enumerator for populating Image Library with contents of MyPictures directory
        private static IEnumerable<TilePhotoInfo> GetMyPicturesImage()
        {
            var di = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
            var fileList = di.GetFiles("*", SearchOption.AllDirectories);
            foreach (var f in fileList.Where(n => (n.Length <= IMAGES_MAXSIZE) && 
                                            (new String[] {".jpg", ".jpeg", ".png", ".gif"}.Contains(n.Extension.ToLower()))))
            {
                yield return new TilePhotoInfo(Path.GetFileNameWithoutExtension(f.Name), f.FullName);
            }
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            lbStatus.Items.Clear();
        }
        private void rbDelete_CheckedChanged(object sender, EventArgs e)
        {
            btnGo.Enabled = true;
        }
        private void rbCreate_CheckedChanged(object sender, EventArgs e)
        {
            btnGo.Enabled = true;
            foreach (CheckBox chkbox in requiredStorageCheckBoxes)
                chkbox.Enabled = rbCreate.Checked;
        }
        private void rbCreateLibrary_CheckedChanged(object sender, EventArgs e)
        {
            btnGo.Enabled = true;
            foreach (CheckBox chkbox in imagesListCheckBoxes)
                chkbox.Enabled = rbCreateLibrary.Checked;
        }

        private void StorageManager_Load(object sender, EventArgs e)
        {
            imagesListCheckBoxes = (from chkbox in this.Controls.OfType<CheckBox>()
                                    where chkbox.Name.StartsWith("chkImage")
                                    orderby chkbox.Name
                                    select chkbox).ToList();
            for (int i = 0; i < Math.Min(imagesListCheckBoxes.Count, imagesList.Count); i++)
            {
                imagesListCheckBoxes[i].Visible = true;
                imagesListCheckBoxes[i].Text = imagesList[i].ContainerName;
                imagesList[i].BoundCheckBox = imagesListCheckBoxes[i];
            }

            requiredStorageCheckBoxes = (from chkbox in this.Controls.OfType<CheckBox>()
                                         where chkbox.Name.StartsWith("chkRequired")
                                         orderby chkbox.Name
                                         select chkbox).ToList();
        }
    }
}