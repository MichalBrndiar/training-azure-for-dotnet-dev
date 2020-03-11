using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace TrainingAzure.AzureFunction
{
    public static class TimerFunction
    {
        [FunctionName("TimerFunction")]
        public static void Run(
            [TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, 
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
