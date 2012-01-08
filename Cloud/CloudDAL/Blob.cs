using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.Diagnostics;

namespace CloudDAL
{
    public class BlobAccessor
    {
        private CloudBlobClient _authBlobClient = null;

        public const String ImageInputContainer = "imageinput";
        public const String SliceInputContainer = "sliceinput";
        public const String SliceOutputContainer = "sliceoutput";
        public const String ImageOutputContainer = "imageoutput";

        public BlobAccessor(String connectionString)
        {
            _authBlobClient = CloudStorageAccount.Parse(connectionString).CreateCloudBlobClient();
        }

        // get list of containers and optional metadata key value
        public IEnumerable<KeyValuePair<Uri, String>> GetContainerUrisAndDescriptions(String metadataTag = null)
        {
            var containers = _authBlobClient.ListContainers("", ContainerListingDetails.Metadata);

            // get list of containers with metadata tag set (or all of them if there is no metadata tag parameter)
            return from c in containers
                   let descTag = (metadataTag == null) ? null : HttpUtility.UrlDecode(c.Metadata[metadataTag.ToLower()])
                   where (descTag != null) || (metadataTag == null)
                   select new KeyValuePair<Uri, String>(c.Uri, descTag);
        }

        // get list of images given blob container URI
        public IEnumerable<Uri> RetrieveImageUris(Uri containerUri, String prefix = "")
        {
            return RetrieveImageUris(containerUri.AbsolutePath.Trim(new char[] { '/' }), prefix);
        }

        // get list of images give blob container name
        public IEnumerable<Uri> RetrieveImageUris(String containerName, String prefix = "")
        {
            IEnumerable<Uri> blobNames = new List<Uri>();
            try
            {
                // get unescaped names of blobs that have image content
                var contentTypes = new String[] { "image/jpeg", "image/gif", "image/png" };
                blobNames = from CloudBlob b in
                                _authBlobClient.ListBlobsWithPrefix(String.Format("{0}/{1}", containerName, prefix),
                                                                    new BlobRequestOptions() { UseFlatBlobListing = true })
                            where contentTypes.Contains(b.Properties.ContentType)
                            select b.Uri;
            }
            catch (Exception e)
            {
                Trace.TraceError("BlobUri: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                    String.Format("{0}/{1}", containerName, prefix),
                    e.GetType(),
                    e.Message,
                    e.StackTrace,
                    Environment.NewLine);
                throw;
            }

            return blobNames;
        }

        // get image bytes for given blob URI
        public byte[] RetrieveImage(Uri fullUri)
        {
            return _authBlobClient.GetBlobReference(fullUri.ToString()).DownloadByteArray();
        }

        // try retrieve
        public Boolean TryRetrieveImage(Uri fullUri, out byte[] imgBytes)
        {
            Boolean retVal = false;
            try
            {
                imgBytes = _authBlobClient.GetBlobReference(fullUri.ToString()).DownloadByteArray();
                retVal = true;
            }
            catch (StorageClientException scex)
            {
                if (scex.ErrorCode == StorageErrorCode.BlobNotFound)
                {
                    imgBytes = null;
                    retVal = false;
                }
                else
                    throw;
            }
            return retVal;
        }

        // iterator for images in given list of URIs
        public IEnumerable<byte[]> RetrieveImages(IEnumerable<Uri> Uris)
        {
            foreach (var uri in Uris)
            {
                var imgBytes = this.RetrieveImage(uri);
                yield return imgBytes;
            }
        }

        // delete image given URI
        public void DeleteImage(Uri fullUri)
        {
            try
            {
                CloudBlockBlob blobRef = new CloudBlockBlob(fullUri.ToString(), _authBlobClient);
                blobRef.DeleteIfExists();
            }
            catch (Exception e)
            {
                Trace.TraceWarning("BlobUri: {0}{4}Exception: {1}{4}Message: {2}{4}Trace: {3}",
                    fullUri,
                    e.GetType(),
                    e.Message,
                    e.StackTrace,
                    Environment.NewLine);
            }
        }

        // store image with given URI (with optional metadata)
        public Uri StoreImage(Byte[] bytes, Uri fullUri, NameValueCollection metadata = null)
        {
            String accessSignature = String.Empty;

            CloudBlockBlob blobRef = new CloudBlockBlob(fullUri.ToString(), _authBlobClient);
            blobRef.Properties.ContentType = "image/jpeg";

            // add the metadata
            if (metadata != null) blobRef.Metadata.Add(metadata);

            // upload the image
            blobRef.UploadByteArray(bytes);

            return blobRef.Uri;
        }

        // store image with given container and image name (with optional metadata)
        public Uri StoreImage(Byte[] bytes, String container, String imageName, NameValueCollection metadata = null)
        {
            var containerRef = _authBlobClient.GetContainerReference(container);
            containerRef.CreateIfNotExist();

            return StoreImage(bytes, new Uri(String.Format("{0}/{1}", containerRef.Uri, imageName)), metadata);
        }

        public DateTime GetLastModifiedDate(Uri fullUri)
        {
            CloudBlockBlob blobRef = new CloudBlockBlob(fullUri.ToString());
            return blobRef.Properties.LastModifiedUtc;
        }

        public Uri CreateThumbnailContainer(Uri container, Byte size)
        {
            String newContainer = String.Format("{0}-{1:00}", container.AbsoluteUri, size);

            var containerRef = _authBlobClient.GetContainerReference(newContainer);
            containerRef.CreateIfNotExist();

            return containerRef.Uri;
        }
    }
}
