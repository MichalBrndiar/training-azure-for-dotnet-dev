using Newtonsoft.Json;
using System;

namespace TrainingAzure.WebApp.Services
{
    public class ImageInfo
    {
        public Uri Uri { get; set; }
        public string Name { get; set; }
        public string Gender;
        public double? Age { get; set; }
        public double Anger { get; set; }
        public double Contempt { get; set; }
        public double Disgust { get; set; }
        public double Fear { get; set; }
        public double Happiness { get; set; }
        public double Neutral { get; set; }
        public double Sadness { get; set; }
        public double Surprise { get; set; }
    }
}
