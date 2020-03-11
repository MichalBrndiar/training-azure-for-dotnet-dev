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
    public class ImageAnalyzer
    {
        private IFaceClient CreateFaceClient()
        {
            var endpoint = Environment.GetEnvironmentVariable("FaceApiEndpoint");
            var key = Environment.GetEnvironmentVariable("FaceApiKey");

            return CreateFaceClient(endpoint, key);
        }

        private IFaceClient CreateFaceClient(string endpoint, string key)
        {
            return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
        }

        public async Task<ImageAnalysis> DetectFaces(Stream image)
        {
            var client = CreateFaceClient();
            var attributes = new List<FaceAttributeType> {
                FaceAttributeType.Accessories,
                FaceAttributeType.Age,
                FaceAttributeType.Blur,
                FaceAttributeType.Emotion,
                FaceAttributeType.Exposure,
                FaceAttributeType.FacialHair,
                FaceAttributeType.Gender,
                FaceAttributeType.Glasses,
                FaceAttributeType.Hair,
                FaceAttributeType.HeadPose,
                FaceAttributeType.Makeup,
                FaceAttributeType.Noise,
                FaceAttributeType.Occlusion,
                FaceAttributeType.Smile };

            var result = await client.Face.DetectWithStreamAsync(image,
                returnFaceAttributes: attributes,
                recognitionModel: RecognitionModel.Recognition02);

            var analysis = new ImageAnalysis { DetectedFaces = result };

            return analysis;
        }
    }
}
