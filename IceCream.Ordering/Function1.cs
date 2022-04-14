// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Dynamic;
using System.Net.Http;

namespace IceCream.Ordering
{
    public static class Function1
    {

        [FunctionName(nameof(Counter))]
        public static async Task Counter([EntityTrigger] IDurableEntityContext ctx, ILogger log)
        {
            var currentValue = ctx.GetState<Dictionary<string, FileTriplet>>() ?? new Dictionary<string, FileTriplet>();
            var input = ctx.GetInput<EventGridEvent>();

            var fileParts = input.Subject.Split('/', StringSplitOptions.RemoveEmptyEntries).Last().Split("-");
            var prefix = fileParts[0];
            var type = fileParts[1];

            var dataContent = JsonConvert.DeserializeObject<GridData>(JsonConvert.SerializeObject(input.Data));

            log.LogInformation($"{dataContent.url}");

            if (!currentValue.ContainsKey(prefix))
            {
                currentValue.Add(prefix, new FileTriplet());
            }

            if (currentValue[prefix].Add(type, dataContent.url) == 3)
            {
                // invoke remote url


                using var httpCli = new HttpClient();
#pragma warning disable S1075 // URIs should not be hardcoded
                var uri = "https://serverlessohmanagementapi.trafficmanager.net/api/order/combineOrderContent";
#pragma warning restore S1075 // URIs should not be hardcoded

                log.LogInformation(JsonConvert.SerializeObject(currentValue[prefix]));

                httpCli.DefaultRequestHeaders.Add("ContentType", "application/json");
                httpCli.DefaultRequestHeaders.Add("Accept", "application/json");
                var rep = await httpCli.PostAsJsonAsync(uri, currentValue[prefix]);

                log.LogInformation($"HTTP Code : {rep.StatusCode} for {prefix}");

                currentValue.Remove(prefix);
            }
            ctx.SetState(currentValue);
        }

        // 
        [FunctionName(nameof(NewFileReceived))]
        public static async Task NewFileReceived([EventGridTrigger] EventGridEvent eventGridEvent, [DurableClient] IDurableEntityClient entityClient, ILogger log)
        {
            var body = JsonConvert.SerializeObject(eventGridEvent);
            log.LogInformation(body);

            // string instanceId = await starter.StartNewAsync(nameof(Counter), eventGridEvent);
            await entityClient.SignalEntityAsync(new EntityId(nameof(Counter), "key"), eventGridEvent.EventType, eventGridEvent);

        }
    }
}
