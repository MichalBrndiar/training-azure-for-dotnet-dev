using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure.CognitiveServices.Vision;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Newtonsoft.Json;
using TrainingAzure.AzureFunction.Services;

namespace TrainingAzure.AzureFunction
{
    public static class ImageAnalysisFunction
    {
        [FunctionName("ImageAnalysisFunction")]
        public static async Task Run(
            [BlobTrigger("images/{name}", Connection = "TrainingAzureStorage")]
            CloudBlockBlob blob,
            string name,
            ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Uri: {blob.Uri} Bytes");

            await PerformAnalysisAsync(blob, log);
        }

        private static async Task PerformAnalysisAsync(CloudBlockBlob blob, ILogger log)
        {
            var imageId = blob.Name;

            using (var ms = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(ms);
                ms.Position = 0;

                var analyzer = new ImageAnalyzer();
                var analysis = await analyzer.DetectFaces(ms);
                analysis.Id = imageId;

                var json = JsonConvert.SerializeObject(analysis, Formatting.Indented);
                log.LogInformation($"Detected faces: {json}");

                var store = new ImageAnalysisStore();
                await store.StoreAsync(analysis);
            }
        }
    }
}
