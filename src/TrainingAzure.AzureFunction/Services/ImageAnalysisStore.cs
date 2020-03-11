using Azure.Cosmos;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainingAzure.AzureFunction.Services
{

    public class ImageAnalysisStore
    {
        public async Task StoreAsync(ImageAnalysis analysis)
        {
            var databaseId = Environment.GetEnvironmentVariable("ImageDbDatabaseId");
            var containerId = Environment.GetEnvironmentVariable("ImageDbContainerId");
            var client = CreateClient();
            await client.CreateDatabaseIfNotExistsAsync(databaseId);

            var container = client.GetContainer(databaseId, containerId);

            var query = new QueryDefinition($"SELECT * FROM c WHERE c.id = '{analysis.Id}'");
            var sequence = container.GetItemQueryIterator<ImageAnalysis>(query);
            var items = await sequence.ToListAsync();
            if (items.Count > 0)
            {
                await container.UpsertItemAsync(analysis);
                //await container.ReplaceItemAsync(analysis, analysis.Id);
            }
            else
            {
                await container.CreateItemAsync(analysis);
            }
        }

        private CosmosClient CreateClient()
        {
            var connString = Environment.GetEnvironmentVariable("ImageDbConnectionString");

            return new CosmosClient(connString);
        }
    }
}
