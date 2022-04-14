using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Company.Function
{
    public static class DurableFunctionsOrchestration
    {
        [FunctionName("DurableFunctionsOrchestration")]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {


            log.LogInformation("start orchestrator");

            var outputs = new List<string>();

            log.LogInformation("wait");

            var gate1 = context.WaitForExternalEvent<string>(context.InstanceId+"-OrderHeaderDetails.csv");
            var gate2 = context.WaitForExternalEvent<string>(context.InstanceId+"-OrderLineItems.csv");
            var gate3 = context.WaitForExternalEvent<string>(context.InstanceId+"-ProductInformation.csv");

            await Task.WhenAll(gate1, gate2, gate3);

            log.LogInformation("all files received !!!!");

            log.LogInformation("task 1 " + gate1.Result);

            var data = new
            {
                orderHeaderDetailsCSVUrl = gate1.Result,
                orderLineItemsCSVUrl = gate2.Result,
                productInformationCSVUrl = gate3.Result
            };

            await context.CallActivityAsync("DurableFunctionsOrchestration_Insert_Json", data);

        }

        [FunctionName("DurableFunctionsOrchestration_Insert_Json")]
        public static async Task InsertJson([ActivityTrigger] string mergeReqData, ILogger log)
        {

            dynamic data = JsonConvert.DeserializeObject(mergeReqData);
            string orderHeaderDetailsCSVUrl = data.OrderHeaderDetailsCSVUrl;
            string orderLineItemsCSVUrl = data.OrderLineItemsCSVUrl;
            string productInformationCSVUrl = data.ProductInformationCSVUrl;

            log.LogInformation(orderHeaderDetailsCSVUrl);
            log.LogInformation(orderLineItemsCSVUrl);
            log.LogInformation(productInformationCSVUrl);

            HttpClient httpclient = new HttpClient(); 

            log.LogInformation(mergeReqData);
            httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var httpResponseMessage = await httpclient.PostAsync("https://serverlessohmanagementapi.trafficmanager.net/api/order/combineOrderContent", new StringContent(mergeReqData));


            string resp = await httpResponseMessage.Content.ReadAsStringAsync();

            log.LogInformation(resp);

        }

        [FunctionName("FileEvent")]
        public static async Task Run([EventGridTrigger]EventGridEvent eventGridEvent, [DurableClient] IDurableOrchestrationClient starter, ILogger log)
        {
            dynamic data = JsonConvert.DeserializeObject(eventGridEvent.Data.ToString());
            string url = data?.url;

            log.LogInformation(eventGridEvent.Data.ToString());

            log.LogInformation(url);


            var split = url.Split("/");

            var filename = split[split.Length - 1];

            var orchestratorId = filename.Split("-")[0];

            var existingInstance = (await starter.GetStatusAsync(orchestratorId));

            if (existingInstance == null
                || existingInstance.RuntimeStatus.ToString() == "Completed" 
                || existingInstance.RuntimeStatus.ToString() == "Failed" 
                || existingInstance.RuntimeStatus.ToString() == "Terminated") {
                
                await starter.StartNewAsync("DurableFunctionsOrchestration", orchestratorId);
                log.LogInformation($"Orchestrator with ID {orchestratorId} started");

            }
            else {
                log.LogInformation($"Orchestrator with ID {orchestratorId} already exist");
            }

            await starter.RaiseEventAsync(orchestratorId, filename, url);


        }
    
    }
}