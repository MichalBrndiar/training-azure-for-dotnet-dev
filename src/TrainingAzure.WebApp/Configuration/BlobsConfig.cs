using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainingAzure.WebApp.Configuration
{
    public class BlobsConfig
    {
        public string StorageName { get; set; }
        public string StorageKey { get; set; }
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
    }
}
