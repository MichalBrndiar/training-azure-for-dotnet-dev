using Azure.Cosmos;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingAzure.WebApp.Configuration;

namespace TrainingAzure.WebApp.Services
{
    public class ImageService
    {
        private readonly AzureConfig config;
        private const string DatabaseId = "imagesDb";
        private const string ContainerId = "imagesContainer";
        private readonly ILogger<ImageService> log;

        public ImageService(IOptionsMonitor<AzureConfig> config, ILogger<ImageService> log)
        {
            this.config = config.CurrentValue;
            this.log = log;
        }

        public async Task<Uri> UploadAsync(string imageName, Stream stream)
        {
            BlobContainerClient container = CreateBlobClient();

            BlobClient blobClient = container.GetBlobClient(imageName);

            BlobContentInfo blobInfo = await blobClient.UploadAsync(stream);

            return blobClient.Uri;
        }

        public async Task DeleteAsync(string imageName)
        {
            BlobContainerClient container = CreateBlobClient();

            BlobClient blobClient = container.GetBlobClient(imageName);
            await blobClient.DeleteIfExistsAsync();
        }


        public async IAsyncEnumerable<ImageInfo> GetAllAsync()
        {
            BlobContainerClient container = CreateBlobClient();

            await foreach (BlobItem blob in container.GetBlobsAsync())
            {
                var info = GetImageInfo(blob.Name);
                log.LogDebug($"Image {blob.Name} retrieved successfully");
                
                var analysis = await GetImageAnalysis(blob.Name);

                var face = analysis?.DetectedFaces?.FirstOrDefault();
                if (face != null)
                {
                    log.LogDebug($"Analysis for image {blob.Name} retrieved successfully");

                    info.Gender = face.FaceAttributes.Gender.ToString();
                    info.Age = face.FaceAttributes.Age;
                    info.Anger = face.FaceAttributes.Emotion.Anger;
                    info.Contempt = face.FaceAttributes.Emotion.Contempt;
                    info.Disgust = face.FaceAttributes.Emotion.Disgust;
                    info.Fear = face.FaceAttributes.Emotion.Fear;
                    info.Happiness = face.FaceAttributes.Emotion.Happiness;
                    info.Neutral = face.FaceAttributes.Emotion.Neutral;
                    info.Sadness = face.FaceAttributes.Emotion.Sadness;
                    info.Surprise = face.FaceAttributes.Emotion.Surprise;
                }

                yield return info;
            }
        }

        private async Task<ImageAnalysis> GetImageAnalysis(string imageName)
        {
            CosmosClient client = CreateCosmosClient();

            CosmosContainer container = client.GetContainer(DatabaseId, ContainerId);

            QueryDefinition query = new QueryDefinition($"SELECT * FROM c WHERE c.id = '{imageName}'");
            
            var sequence = container.GetItemQueryIterator<ImageAnalysis>(query);
            
            return await sequence.FirstOrDefaultAsync();
        }

        private ImageInfo GetImageInfo(string blobName)
        {
            BlobContainerClient container = CreateBlobClient();
            string sasToken = GetSasTokenAsync();

            BlobClient blobClient = container.GetBlobClient(blobName);
            Uri blobUri = blobClient.Uri;
            Uri uri = new UriBuilder
            {
                Scheme = blobUri.Scheme,
                Host = blobUri.Host,
                Path = blobUri.PathAndQuery,
                Query = sasToken
            }.Uri;

            return
                new ImageInfo
                {
                    Name = blobName,
                    Uri = uri
                };
        }

        private CosmosClient CreateCosmosClient()
        {
            return new CosmosClient(this.config.CosmosDb.ConnectionString);
        }

        private BlobContainerClient CreateBlobClient()
        {
            BlobContainerClient container = new BlobContainerClient(
               config.Blobs.ConnectionString,
               config.Blobs.ContainerName);

            return container;
        }

        private string GetSasTokenAsync()
        {
            BlobServiceClient service = new BlobServiceClient(config.Blobs.ConnectionString);
            //UserDelegationKey key = await service.GetUserDelegationKeyAsync(DateTime.UtcNow, DateTime.UtcNow.AddMinutes(1));

            StorageSharedKeyCredential cred = new StorageSharedKeyCredential(this.config.Blobs.StorageName, this.config.Blobs.StorageKey);

            BlobSasBuilder sasBuilder = new BlobSasBuilder();
            sasBuilder.SetPermissions(BlobSasPermissions.Read);
            sasBuilder.StartsOn = DateTime.UtcNow;
            sasBuilder.ExpiresOn = DateTime.UtcNow.AddMinutes(1);
            sasBuilder.BlobContainerName = this.config.Blobs.ContainerName;

            string sasToken = sasBuilder.ToSasQueryParameters(cred).ToString();

            return sasToken;
        }
    }
}
