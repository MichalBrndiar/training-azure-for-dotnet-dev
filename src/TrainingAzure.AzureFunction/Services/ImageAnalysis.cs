using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TrainingAzure.AzureFunction.Services
{
    public class ImageAnalysis
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public ICollection<DetectedFace> DetectedFaces { get; set; }
    }
}
