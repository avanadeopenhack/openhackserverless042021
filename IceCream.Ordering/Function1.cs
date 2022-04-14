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

            if (!currentValue.ContainsKey(prefix))
            {
                currentValue.Add(prefix, new FileTriplet());
            }
            var triplet = currentValue[prefix];

            triplet.Add(type, dataContent.url);
            if (triplet.IsFull())
            {
                // invoke remote url


                using var httpCli = new HttpClient();
                
#pragma warning disable S1075 // URIs should not be hardcoded
                var uri = "https://serverlessohmanagementapi.trafficmanager.net/api/order/combineOrderContent";
#pragma warning restore S1075 // URIs should not be hardcoded

                log.LogInformation(JsonConvert.SerializeObject(triplet));

                log.LogWarning($"Pushing for key {prefix}");
                // var rep = await httpCli.PostAsJsonAsync("", triplet);
                var rep = await httpCli.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(triplet), System.Text.Encoding.UTF8, "application/json"));
                var content = await rep.Content.ReadAsStringAsync();
                log.LogWarning($"HTTP Code : {rep.StatusCode} for {prefix}");
                log.LogError($"{content}");

                string? funcSaveUrl = Environment.GetEnvironmentVariable("functionUrl");
                log.LogInformation($"Function URL : {funcSaveUrl}");
                rep = await httpCli.PostAsync(funcSaveUrl, new StringContent(content, System.Text.Encoding.UTF8, "application/json"));
                log.LogError($"{content}");

                currentValue.Remove(prefix);
            }
            ctx.SetState(currentValue);
        }

        // 
        [FunctionName(nameof(NewFileReceived))]
        public static async Task NewFileReceived([EventGridTrigger] EventGridEvent eventGridEvent, [DurableClient] IDurableEntityClient entityClient, ILogger log)
        {
            await entityClient.SignalEntityAsync(new EntityId(nameof(Counter), "key"), eventGridEvent.EventType, eventGridEvent);
        }
    }
}
